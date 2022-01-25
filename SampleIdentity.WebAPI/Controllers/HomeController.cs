using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleIdentity.Core.Services.Account.Misc;
using SampleIdentity.WebAPI.Filters;
using System.Net;

namespace SampleIdentity.WebAPI.Controllers
{
    [Authorize]
    [Route("api/Home")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            return Ok("Done");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthenticationFilter(SystemRoles.SuperAdministrator)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post()
        {
            return Ok("Done");
        }

    }
}
