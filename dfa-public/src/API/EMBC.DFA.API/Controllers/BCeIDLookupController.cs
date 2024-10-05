using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using EMBC.DFA.API.ConfigurationModule.Models.Dynamics;
using EMBC.DFA.API.Services;
using EMBC.Gov.BCeID;
using Grpc.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bceid = EMBC.Gov.BCeID;

namespace EMBC.DFA.API.Controllers
{
    [Route("api/bceid")]
    [ApiController]
    [Authorize]
    public class BCeIDLookupController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ILogger<BCeIDLookupController> logger;
        private readonly IConfigurationHandler handler;
        private readonly IUserService userService;
        private readonly IBCeIDBusinessQuery bceidQuery;

        public BCeIDLookupController(
            IMapper mapper,
            ILogger<BCeIDLookupController> logger,
            IConfigurationHandler handler,
            IUserService userService,
            IBCeIDBusinessQuery query)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.bceidQuery = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <summary>
        /// Lookup your own BCeID Business Account, using logged in credentials
        /// NOTE to API clients: remember to check "isValidResponse"
        /// </summary>
        /// <returns>BCeIDBusiness object</returns>
        [HttpGet("self")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bceid.BCeIDBusiness>> GetBCeIDSelfInfo()
        {
            try
            {
                var userData = userService.GetJWTokenData();

                if (userData == null)
                {
                    var errResponse = new BCeIDBusiness() { IsValidResponse = false, ResponseErrorMsg = "JWT Token bad - Authentication missing" };
                    return Ok(errResponse);
                }

                var userGuid = userData.bceid_user_guid;
                var bceidData = await this.bceidQuery.ProcessBusinessQuery(userGuid);

                if (bceidData == null)
                {
                    var errResponse = new BCeIDBusiness() { IsValidResponse = false, ResponseErrorMsg = "BCeID Self not found" };
                    return Ok(errResponse);
                }

                bceidData.IsValidResponse = true;
                return Ok(bceidData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lookup a different BCeID Business Account
        /// NOTE to API clients: remember to check "isValidResponse"
        /// </summary>
        /// <param name="userId">The other Business BCeID user to search for</param>
        /// <returns>BCeIDBusiness object</returns>
        [HttpGet("other")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bceid.BCeIDBusiness>> GetBCeIDOtherInfo(
            [FromQuery]
            [Required]
            string userId)
        {
            try
            {
                // get current logged in user info
                var userData = userService.GetJWTokenData();

                if (userData == null)
                {
                    var errResponse = new BCeIDBusiness() { IsValidResponse = false, ResponseErrorMsg = "JWT Token bad - Authentication missing" };
                    return Ok(errResponse);
                }

                var userGuid = userData.bceid_user_guid;
                var orgGuid = userData.bceid_business_guid;
                var bceidData = await this.bceidQuery.ProcessBusinessQuery(userGuid, userId);

                if (bceidData == null)
                {
                    var errResponse = new BCeIDBusiness() { IsValidResponse = false, ResponseErrorMsg = "BCeID lookup other not found" };
                    return Ok(errResponse);
                }
                // 2024-09-13 EMCRI-676 waynezen: Don't allow search on BCeID users from a different Organization
                else if (!orgGuid.Equals(bceidData.organizationGuid))
                {
                    var errResponse = new BCeIDBusiness() { IsValidResponse = false, ResponseErrorMsg = "Unauthorized lookup outside of Organization" };
                    return Ok(errResponse);
                }

                bceidData.IsValidResponse = true;
                return Ok(bceidData);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
