using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using EMBC.DFA.API.ConfigurationModule.Models.AuthModels;
using EMBC.DFA.API.ConfigurationModule.Models.Dynamics;
using EMBC.DFA.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using bceid = EMBC.Gov.BCeID;

namespace EMBC.DFA.API.Controllers
{
    [Route("api/contacts")]
    [ApiController]
    [Authorize]

    public class ContactController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IEvacuationSearchService evacuationSearchService;
        private readonly IProfileInviteService profileInviteService;
        private readonly IConfigurationHandler handler;
        private readonly IUserService userService;

        public ContactController(
            IMapper mapper,
            IEvacuationSearchService evacuationSearchService,
            IProfileInviteService profileInviteService,
            IConfigurationHandler handler,
            IUserService userService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.evacuationSearchService = evacuationSearchService ?? throw new ArgumentNullException(nameof(evacuationSearchService));
            this.profileInviteService = profileInviteService ?? throw new ArgumentNullException(nameof(profileInviteService));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("dashboard")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bceid.BCeIDBusiness> GetDashboardContactInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userData = userService.GetJWTokenData();

                if (userData == null) return NotFound("Authentication missing");

                var bceidData = mapper.Map<bceid.BCeIDBusiness>(userData);
                return Ok(bceidData);
            }
            else
            {
                Debug.WriteLine("No Authentication!");
                return Ok();
            }
        }

        /// <summary>
        /// If user isn't authenticated, return NULL
        /// </summary>
        /// <returns>NULL if user isn't authenticated</returns>
        [HttpGet("getlogin")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BceidUserData> GetLoginInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userData = userService.GetJWTokenData();

                return Ok(userData);
            }
            else
            {
                Debug.WriteLine("No Authentication!");
                return Ok(null);
            }
        }

        /// <summary>
        /// Gets the same BCeID user info as getlogin, but also ensures that a dfa_bceidusers record exists
        /// for the current logged in user.
        /// </summary>
        /// <param name="eventMoniker">Brief description of event type; eg. "login", or "submit Application"</param>
        /// <returns>BCeID user info</returns>
        [HttpGet("createaudit")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> CreateAuditEvent(string eventMoniker)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userData = userService.GetJWTokenData();
                dfa_audit_event bceidUser = new dfa_audit_event()
                {
                    dfa_name = userData.name,
                    dfa_bceidbusinessguid = Convert.ToString(userData.bceid_business_guid),
                    dfa_bceiduserguid = Convert.ToString(userData.bceid_user_guid),
                    dfa_eventdate = DateTime.Now,
                    dfa_auditdescription = eventMoniker
                };
                var result = await handler.HandleBCeIDAudit(bceidUser);

                return Ok(result);
            }
            else
            {
                Debug.WriteLine("No Authentication!");
                return Ok("error");
            }
        }
    }
}
