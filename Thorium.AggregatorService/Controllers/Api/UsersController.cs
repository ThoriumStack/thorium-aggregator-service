using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thorium.Aggregator.AuthorizationModule.Filters;
using Thorium.Aggregator.Filters;
using Thorium.Aggregator.Models;
using Thorium.Core.DataTools;
using Thorium.Mvc.Models;
using Thorium.Mvc.Tools;
using Thorium.Users.ServiceContract;

namespace Thorium.Aggregator.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/users")]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    [FlurlExceptionFilter]
    [Authorize]
    public class UsersController : ApiController
    {
        private UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<UserSearchDto>), 200)]
        [ResourcePermission("user", "search")]
        
        public async Task<IActionResult> GetUsers(int pageIndex, int pageSize)
        {
            var users = _userManager.Users
                .Where(c=>c.IsInternal)
                .Select(c => new UserSearchDto
            {
                Id = c.Id,
                Username = c.UserName,
                Telephone = c.PhoneNumber,
            });

            return FromReply(users.Paginate<UserSearchDto, UserSearchDto>( pageIndex, pageSize));
        }
    }
}