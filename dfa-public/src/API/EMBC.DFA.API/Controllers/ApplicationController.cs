using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using EMBC.DFA.API.ConfigurationModule.Models.Dynamics;
using EMBC.DFA.API.ConfigurationModule.Models.PDF;
using EMBC.DFA.API.ConfigurationModule.Models.PDF.PDFService;
using EMBC.DFA.API.Services;
using Google.Protobuf.WellKnownTypes;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace EMBC.DFA.API.Controllers
{
    [Route("api/applications")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IHostEnvironment env;
        private readonly IMapper mapper;
        private readonly IConfigurationHandler handler;
        // 2024-08-11 EMCRI-595 waynezen; BCeID Authentication
        private readonly IUserService userService;
        private readonly PDFServiceHandler pDFServiceHandler;

        public ApplicationController(
            IHostEnvironment env,
            IMapper mapper,
            IConfigurationHandler handler,
            IUserService userService,
            PDFServiceHandler pDFServiceHandler)
        {
            this.env = env;
            this.mapper = mapper;
            this.handler = handler;
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.pDFServiceHandler = pDFServiceHandler;
        }

        private string currentUserId => userService.GetBCeIDBusinessId();

        /// <summary>
        /// Create an application
        /// </summary>
        /// <param name="application">The application information</param>
        /// <returns>application id</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> AddApplication(DFAApplicationStart application)
        {
            // fill in current user
            var userId = userService.GetBCeIDBusinessId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var dfa_appcontact = await handler.HandleGetUser(userId);
            application.ProfileVerification.profileId = dfa_appcontact.Id;

            if (application == null) return BadRequest("Application details cannot be empty.");
            var mappedApplication = mapper.Map<dfa_appapplicationstart_params>(application);
            var tempParms = mapper.Map<temp_dfa_appapplicationstart_params>(application); // TODO: remove

            var applicationId = await handler.HandleApplication(mappedApplication, tempParms);

            // Verify Profile
            application.ProfileVerification.profile.BCeIDBusinessGuid = currentUserId;
            var mappedProfile = mapper.Map<dfa_appcontact>(application.ProfileVerification.profile);
            var msg = await handler.HandleContact(mappedProfile);

            // If no insurance, add signatures
            if (application.AppTypeInsurance.insuranceOption == InsuranceOption.No)
            {
                IEnumerable<dfa_signature> insuranceSignatures = Enumerable.Empty<dfa_signature>();
                if (application.AppTypeInsurance.applicantSignature != null && application.AppTypeInsurance.applicantSignature.signature != null)
                {
                    var primarySignature = new dfa_signature();
                    primarySignature.signature = application.AppTypeInsurance.applicantSignature.signature.Substring(application.AppTypeInsurance.applicantSignature.signature.IndexOf(',') + 1);
                    primarySignature.filename = "primaryApplicantSignatureNoIns";
                    primarySignature.dfa_appapplicationid = applicationId;
                    await handler.HandleSignature(primarySignature);
                }

                if (application.AppTypeInsurance.secondaryApplicantSignature != null && application.AppTypeInsurance.secondaryApplicantSignature.signature != null)
                {
                    var secondarySignature = new dfa_signature();
                    secondarySignature.signature = application.AppTypeInsurance.secondaryApplicantSignature.signature.Substring(application.AppTypeInsurance.secondaryApplicantSignature.signature.IndexOf(',') + 1);
                    secondarySignature.filename = "secondaryApplicantSignatureNoIns";
                    secondarySignature.dfa_appapplicationid = applicationId;
                    await handler.HandleSignature(secondarySignature);
                }
            }
            return Ok(applicationId);
        }

        /// <summary>
        /// Update an application
        /// </summary>
        /// <param name="application">The application information</param>
        /// <returns>application id</returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UpdateApplication(DFAApplicationMain application)
        {
            var e2 = ((ApplicantSubtypeCategories)System.Enum.Parse(typeof(ApplicantSubtypeCategories), "FirstNationCommunity")).ToString();
            var e3 = ((EstimatedPercent)System.Enum.Parse(typeof(EstimatedPercent), "FirstNationCommunity")).ToString();

            if (application == null) return BadRequest("Application details cannot be empty.");

            var userData = userService.GetJWTokenData();
            if (userData == null) return NotFound();

            // 2024-09-17 EMCRI-663 waynezen; handle Primary Contact
            var primeContactIn = mapper.Map<dfa_applicationprimarycontact_params>(application.applicationContacts);

            var contactId = primeContactIn?.dfa_appcontactid;
            if (primeContactIn?.dfa_bceiduserguid != null)
            {
                // saving new Application - try to find existing Primary contact using BCeID user guid
                contactId = await handler.HandlePrimaryContactAsync(primeContactIn);

                // if still no contactId, then error
                if (contactId == null)
                {
                    return BadRequest("Create/update Primary Contact failed");
                }
            }

            var mappedApplication = mapper.Map<dfa_appapplicationmain_params>(application);
            mappedApplication.dfa_applicant = contactId;

            // 2024-09-17 EMCRI-676 waynezen; if no Primary Contact yet, use BCeID guid's of logged in user
            if (mappedApplication?.dfa_bceidbusinessguid == null && primeContactIn?.dfa_bceidbusinessguid == null)
            {
                mappedApplication.dfa_bceidbusinessguid = userData.bceid_business_guid.ToString();
            }
            else
            {
                mappedApplication.dfa_bceidbusinessguid = primeContactIn.dfa_bceidbusinessguid;
            }

            var result = await handler.HandleApplicationUpdate(mappedApplication, null);

            if (application.OtherContact != null)
            {
                foreach (var objContact in application.OtherContact)
                {
                    if (string.IsNullOrEmpty(mappedApplication.dfa_appapplicationid))
                    {
                        objContact.applicationId = Guid.Parse(result);
                    }
                    else
                    {
                        objContact.applicationId = Guid.Parse(mappedApplication.dfa_appapplicationid);
                    }
                    var mappedOtherContact = mapper.Map<dfa_appothercontact_params>(objContact);

                    var resultContact = await handler.HandleOtherContactAsync(mappedOtherContact);
                }
            }
            if (application != null && application.applicationDetails != null && application.applicationDetails.appStatus == ApplicationStageOptionSet.SUBMIT)
            {
                PdfReuest pdfReuest = new PdfReuest
                {
                    PdfApplicationData = GetPdfApplicationData(application),
                    Template = "dfa_application_summary"
                };

                var file = await pDFServiceHandler.GetFileDataAsync(pdfReuest);

                var applicationReviewPDFUpload = BuildApplicationReviewPDFUpload(mappedApplication, file);
                try
                {
                    var mappedFileUpload = mapper.Map<AttachmentEntity>(applicationReviewPDFUpload);
                    var submissionEntity = mapper.Map<SubmissionEntityPDF>(applicationReviewPDFUpload);
                    submissionEntity.documentCollection = Enumerable.Empty<AttachmentEntity>();
                    submissionEntity.documentCollection = submissionEntity.documentCollection.Append<AttachmentEntity>(mappedFileUpload);
                    var fileUploadResult = await handler.HandleFileUploadApplicationPDFAsync(submissionEntity);
                }
                catch (Exception ex)
                {
                    return Ok(ex.Message);
                }
            }
            return Ok(result);
        }

        private ApplicationReviewPDFUpload BuildApplicationReviewPDFUpload(dfa_appapplicationmain_params mappedApplication, byte[] file)
        {
            return new ApplicationReviewPDFUpload
            {
                dfa_appapplicationid = Guid.Parse(mappedApplication.dfa_appapplicationid),
                contentType = "application/pdf",
                fileData = file,
                fileType = FileCategory.AppplicationPDF,
                fileName = $"{mappedApplication.dfa_appapplicationid}.pdf",
                fileDescription = "Appplication PDF",
                uploadedDate = DateTime.Now.ToString()
            };
        }

        public PdfApplicationData GetPdfApplicationData(DFAApplicationMain application)
        {
            var contacts = new Contact[]
            {
                    new Contact
                    {
                        FirstName = "Karim", LastName = "Hass", CellPhone = "222233", BusinessPhone = "44444444", Email = "Karim@12332.com", JobTitle = "Co-Owner", Notes = "notes"
                    },
                    new Contact
                    {
                        FirstName = "Karim1", LastName = "Hass1", CellPhone = "2222331", BusinessPhone = "444444441", Email = "Karim@123321.com", JobTitle = "Co-Owner1", Notes = "notes1"
                    }
            };
            var contactText = new StringBuilder();
            contactText.Append($@"<div class='contacts-container' ><table class='contacts' style='width:95%'>");

            contactText.Append($@"<tr style='background-color: #415a88;color: #fff;'>
                         <th>First Name</th><th>Last Name</th><th>Business Phone</th><th>Email</th><th>Cell Phone</th><th>Job Title</th><th>Notes</th></tr>");
            foreach (var contact in contacts)
            {
                contactText.Append($@"<tr>
                        <td>{contact.FirstName}</td>
                        <td>{contact.LastName}</td>
                        <td>{contact.BusinessPhone}</td>   
                        <td>{contact.Email}</td>
                        <td>{contact.CellPhone}</td>
                        <td>{contact.JobTitle}</td>
                        <td>{contact.Notes}</td>
                        </tr>");
            }

            contactText.Append("</table></div>");

            var pdfApplicationData = new PdfApplicationData
            {
                IndigenousGoverningBody = "SmallBusinessOwner",
                DateofDamageFrom = "DateofDamageTo",
                DateofDamageTo = "DateofDamageTo",
                DisasterEvent = "DisasterEvent",
                CauseofDamage = "CauseofDamage ",
                GovernmentType = "GovernmentType",
                OtherGoverningBody = "OtherGoverningBody",
                DescribeYourOrganization = "DescribeYourOrganization",

                //////////// Second section////////////
                DoingBusinessAsDBAName = "DoingBusinessAsDBAName",
                BusinessNumber = "BusinessNumber",
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                City = "City",
                Province = "Province",
                PostalCode = "PostalCode",

                //Primary Contact Details
                FirstName = "FirstName",
                LastName = "LastName",
                Department = "Department",
                BusinessPhone = "BusinessPhone",
                EmailAddress = "EmailAddress",
                CellPhone = "CellPhone",
                JobTitle = "JobTitle",
                // ContactNotes = "ContactNotes",

                //Contacts
                ContactsText = contactText.ToString(),
            };
            return pdfApplicationData;
            //if (System.IO.File.Exists(filename))
            //{
            //    string format = System.IO.File.ReadAllText(filename);
            //    HandlebarsTemplate<object, object> handlebar = GetHandlebarsTemplate(format);

            //    handlebar = Handlebars.Compile(format);
            //    var html = handlebar(rawdata);

            //    var doc = new HtmlToPdfDocument()
            //    {
            //        GlobalSettings = {
            //            PaperSize = PaperKind.Letter,
            //            Orientation = Orientation.Portrait,
            //            Margins = new MarginSettings(5.0,5.0,5.0,5.0)
            //        },

            //        Objects = {
            //            new ObjectSettings()
            //            {
            //                HtmlContent = html
            //            }
            //        }
            //    };
            //    try
            //    {
            //        var pdf = _generatePdf.Convert(doc);
            //        string bitString = BitConverter.ToString(pdf);

            //        return File(pdf, "application/pdf", "test.pdf");
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e, "ERROR rendering PDF");
            //        _logger.LogError(template);
            //        _logger.LogError(html);
            //    }
            //    return Content(html, "text/html", Encoding.UTF8);
            }

        //public PdfApplicationData GetPdfApplicationData()
        //{
        //    var contacts = new Contact[]
        //    {
        //            new Contact
        //            {
        //                FirstName = "Karim", LastName = "Hass", CellPhone = "222233", BusinessPhone = "44444444", Email = "Karim@12332.com", JobTitle = "Co-Owner", Notes = "notes"
        //            },
        //            new Contact
        //            {
        //                FirstName = "Karim1", LastName = "Hass1", CellPhone = "2222331", BusinessPhone = "444444441", Email = "Karim@123321.com", JobTitle = "Co-Owner1", Notes = "notes1"
        //            }
        //    };
        //    var contactText = new StringBuilder();
        //    contactText.Append($@"<div class='contacts-container' ><table class='contacts' style='width:95%'>");

        //    contactText.Append($@"<tr style='background-color: #415a88;color: #fff;'>
        //                 <th>First Name</th><th>Last Name</th><th>Business Phone</th><th>Email</th><th>Cell Phone</th><th>Job Title</th><th>Notes</th></tr>");
        //    foreach (var contact in contacts)
        //    {
        //        contactText.Append($@"<tr>
        //                <td>{contact.FirstName}</td>
        //                <td>{contact.LastName}</td>
        //                <td>{contact.BusinessPhone}</td>
        //                <td>{contact.Email}</td>
        //                <td>{contact.CellPhone}</td>
        //                <td>{contact.JobTitle}</td>
        //                <td>{contact.Notes}</td>
        //                </tr>");
        //    }

        //    contactText.Append("</table></div>");

        //    var pdfApplicationData = new PdfApplicationData
        //    {
        //        IndigenousGoverningBody = "SmallBusinessOwner",
        //        DateofDamageFrom = "DateofDamageTo",
        //        DateofDamageTo = "DateofDamageTo",
        //        DisasterEvent = "DisasterEvent",
        //        CauseofDamage = "CauseofDamage ",
        //        GovernmentType = "GovernmentType",
        //        OtherGoverningBody = "OtherGoverningBody",
        //        DescribeYourOrganization = "DescribeYourOrganization",

        //        //////////// Second section////////////
        //        DoingBusinessAsDBAName = "DoingBusinessAsDBAName",
        //        BusinessNumber = "BusinessNumber",
        //        AddressLine1 = "AddressLine1",
        //        AddressLine2 = "AddressLine2",
        //        City = "City",
        //        Province = "Province",
        //        PostalCode = "PostalCode",

        //        //Primary Contact Details
        //        FirstName = "FirstName",
        //        LastName = "LastName",
        //        Department = "Department",
        //        BusinessPhone = "BusinessPhone",
        //        EmailAddress = "EmailAddress",
        //        CellPhone = "CellPhone",
        //        JobTitle = "JobTitle",
        //        // ContactNotes = "ContactNotes",

        //        //Contacts
        //        ContactsText = contactText.ToString(),
        //    };
        //    return pdfApplicationData;
        //}

        //return new NotFoundResult();

        /// <summary>
        /// Get an application by Id
        /// </summary>
        /// <returns> DFAApplicationStart</returns>
        /// <param name="applicationId">The application Id.</param>
        [HttpGet("appstart/byId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DFAApplicationStart>> GetApplicationStart(
            [FromQuery]
            [Required]
            Guid applicationId)
        {
            var dfa_appapplication = await handler.GetApplicationStartAsync(applicationId);
            DFAApplicationStart dfaApplicationStart = new DFAApplicationStart();
            dfaApplicationStart.Id = applicationId;
            dfaApplicationStart.ProfileVerification = mapper.Map<ProfileVerification>(dfa_appapplication);
            dfaApplicationStart.Consent = mapper.Map<Consent>(dfa_appapplication);
            dfaApplicationStart.AppTypeInsurance = mapper.Map<AppTypeInsurance>(dfa_appapplication);
            dfaApplicationStart.OtherPreScreeningQuestions = mapper.Map<OtherPreScreeningQuestions>(dfa_appapplication);

            // Fill in profile
            // 2024-08-11 EMCRI-595 waynezen; BCeID Authentication
            var userId = userService.GetBCeIDBusinessId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var profile = await handler.HandleGetUser(userId);
            dfaApplicationStart.ProfileVerification.profile = profile;
            return Ok(dfaApplicationStart);
        }

        /// <summary>
        /// Get an application main by Id
        /// </summary>
        /// <returns> DFAApplicationMain</returns>
        /// <param name="applicationId">The application Id.</param>
        [HttpGet("appmain/byId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DFAApplicationMain>> GetApplicationMain(
            [FromQuery]
            [Required]
            Guid applicationId)
        {
            try
            {
                // 2024-08-11 EMCRI-595 waynezen; BCeID Authentication
                var userData = userService.GetJWTokenData();
                if (userData == null) return NotFound();

                var dfa_appapplication = await handler.GetApplicationMainAsync(applicationId);
                DFAApplicationMain dfaApplicationMain = new DFAApplicationMain();
                dfaApplicationMain.Id = applicationId;
                dfaApplicationMain.applicationDetails = mapper.Map<ApplicationDetails>(dfa_appapplication);

                // 2024-09-24 EMCRI-663; get primary contact info
                if (dfa_appapplication?._dfa_applicant_value != null)
                {
                    var primeContactIn = await handler.HandleGetPrimaryContactAsync(dfa_appapplication._dfa_applicant_value);
                    // convert Dynamics DTO to UI DTO
                    var appContact = mapper.Map<ApplicationContacts>(primeContactIn);
                    // add in a few extra fields from the Application -> Contacts screen
                    mapper.Map<dfa_appapplicationmain_retrieve, ApplicationContacts>(dfa_appapplication, appContact);

                    dfaApplicationMain.applicationContacts = appContact;
                }

                return Ok(dfaApplicationMain);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get dfa applications
        /// </summary>
        /// <returns>list of dfa applications</returns>
        [HttpGet("dfaapplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CurrentApplication>>> GetDFAApplications()
        {
            // 2024-08-11 EMCRI-595 waynezen; BCeID Authentication
            var userData = userService.GetJWTokenData();

            if (userData == null) return NotFound();
            var lstApplications = await handler.HandleApplicationList(userData);
            return Ok(lstApplications);
        }

        /// <summary>
        /// get dfa applications
        /// </summary>
        /// <returns>list of dfa applications</returns>
        /// <param name="applicationId">The application Id.</param>
        [HttpGet("dfaapplicationbyID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CurrentApplication>> GetApplicationDetailsForProject([FromQuery]
            [Required]
            Guid applicationId)
        {
            var lstApplications = await handler.HandleApplicationDetails(Convert.ToString(applicationId));
            return Ok(lstApplications);
        }

        /// <summary>
        /// Get the applicant subtype records
        /// </summary>
        /// <returns>applicant subtype records</returns>
        [HttpGet("applicantsubtypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApplicantSubtypes>> GetApplicantSubTypes()
        {
            var lstApplicantSubtypes = new List<ApplicantSubtypes>()
            {
                new ApplicantSubtypes()
                {
                    ID = "1",
                    SubType = GetEnumMemberAttrValue(ApplicantSubtypeCategories.FirstNationCommunity),
                    DFAComment = string.Empty,
                    EstimatePercent = Convert.ToInt32(EstimatedPercent.FirstNationCommunity),
                },
                new ApplicantSubtypes()
                {
                    ID = "2",
                    SubType = GetEnumMemberAttrValue(ApplicantSubtypeCategories.Municipality),
                    DFAComment = string.Empty,
                    EstimatePercent = Convert.ToInt32(EstimatedPercent.FirstNationCommunity),
                },
                new ApplicantSubtypes()
                {
                    ID = "3",
                    SubType = GetEnumMemberAttrValue(ApplicantSubtypeCategories.RegionalDistrict),
                    DFAComment = string.Empty,
                    EstimatePercent = Convert.ToInt32(EstimatedPercent.RegionalDistrict),
                },
                new ApplicantSubtypes()
                {
                    ID = "4",
                    SubType = GetEnumMemberAttrValue(ApplicantSubtypeCategories.OtherLocalGovernmentBody),
                    DFAComment = string.Empty,
                    EstimatePercent = Convert.ToInt32(EstimatedPercent.OtherLocalGovernmentBody),
                },
                new ApplicantSubtypes()
                {
                    ID = "5",
                    SubType = GetEnumMemberAttrValue(ApplicantSubtypeCategories.Other),
                    DFAComment = string.Empty,
                    EstimatePercent = Convert.ToInt32(EstimatedPercent.Other),
                }
            };
            return Ok(lstApplicantSubtypes);
        }

        /// <summary>
        /// Get the applicant subtype records
        /// </summary>
        /// <returns>applicant subtype records</returns>
        [HttpGet("applicantsubsubtypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApplicantSubtypeSubCategories> GetApplicantSubSubTypes()
        {
            return Ok(null);
        }

        public static string GetEnumDescription(System.Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public string GetEnumMemberAttrValue<T>(T enumVal)
        {
            var enumType = typeof(T);
            var memInfo = enumType.GetMember(enumVal.ToString());
            var attr = memInfo.FirstOrDefault()?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value;
            }

            return null;
        }
    }

    /// <summary>
    /// Application
    /// </summary>
    public class DFAApplicationStart
    {
        public Guid? Id { get; set; }

        public Consent Consent { get; set; }

        public ProfileVerification ProfileVerification { get; set; }

        public AppTypeInsurance AppTypeInsurance { get; set; }

        public OtherPreScreeningQuestions OtherPreScreeningQuestions { get; set; }
        public bool notifyUser { get; set; }
    }

    public class DFAApplicationMain
    {
        public Guid? Id { get; set; }
        public ApplicationDetails? applicationDetails { get; set; }
        // 2024-09-16 EMCRI-663 waynezen; new fields from application
        public ApplicationContacts applicationContacts { get; set; }
        public ProfileVerification? ProfileVerification { get; set; }
        public OtherContact[]? OtherContact { get; set; }
        public bool deleteFlag { get; set; }
        public bool notifyUser { get; set; }
    }

    public class CurrentApplication
    {
        public string ApplicationId { get; set; }
        public string EventId { get; set; }
        public string ApplicationType { get; set; }
        public string DamagedAddress { get; set; }
        public string CaseNumber { get; set; }
        public string DateOfDamage { get; set; }
        public string DateOfDamageTo { get; set; }
        public string PrimaryApplicantSignedDate { get; set; }
        public string DateFileClosed { get; set; }
        public string Status { get; set; }
        public string Stage { get; set; }
        public List<StatusBar> StatusBar { get; set; }
        public string StatusLastUpdated { get; set; }
        public bool IsErrorInStatus { get; set; }
        public bool? floodDamage { get; set; }
        public bool? landslideDamage { get; set; }
        public bool? stormDamage { get; set; }
        public bool? wildfireDamage { get; set; }
        public bool? otherDamage { get; set; }
        public bool? eligibleGST { get; set; }
        public string? otherDamageText { get; set; }
        public string StatusColor { get; set; }
        public bool IsProjectSubmission { get; set; }
    }

    public class ApplicantSubtypes
    {
        public string ID { get; set; }
        public string SubType { get; set; }
        public int EstimatePercent { get; set; }
        public string DFAComment { get; set; }
    }

    public class StatusBar
    {
        public string Status { get; set; }
        public string Stage { get; set; }
        public bool IsCompleted { get; set; }
        public bool CurrentStep { get; set; }
        public bool IsFinalStep { get; set; }
        public bool IsErrorInStatus { get; set; }
        public string StatusColor { get; set; }
    }
}
