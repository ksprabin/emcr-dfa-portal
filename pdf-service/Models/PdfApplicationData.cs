
using System.Collections.Generic;

namespace pdfservice.Models
{
    public class PdfApplicationData
    {
        public string IndigenousGoverningBody { get; set; }
        public string DateofDamageFrom { get; set; }
        public string DateofDamageTo { get; set; }
        public string DisasterEvent { get; set; }
        public string CauseofDamage { get; set; }
        public string GovernmentType { get; set; }
        public string OtherGoverningBody { get; set; }
        public string DescribeYourOrganization { get; set; }
        public string DoingBusinessAsDBAName { get; set; }
        public string BusinessNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string BusinessPhone { get; set; }
        public string EmailAddress { get; set; }
        public string CellPhone { get; set; }
        public string JobTitle { get; set; }
        public List<Contact> Contacts { get; set; }
        public string ContactsText { get; set; }
    }
    public class PdfReuest
    {
        public PdfApplicationData PdfApplicationData { get; set; }
        public string Template { get; set; }
    }
}
