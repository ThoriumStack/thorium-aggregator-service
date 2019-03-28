using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Thorium.Aggregator.AuthorizationModule.Controllers
{
    [Route("v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class AuthorizationController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.GetSubjectId();

            var perms = _authorizationService.GetUserPermissions(userId);

            return Ok(perms);

        }
    }
}