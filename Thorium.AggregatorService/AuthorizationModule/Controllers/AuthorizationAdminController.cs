using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Thorium.Aggregator.AuthorizationModule.Attributes;
using Thorium.Aggregator.AuthorizationModule.Filters;
using Thorium.Aggregator.AuthorizationModule.Models;
using Thorium.Aggregator.AuthorizationModule.ViewModels;
using Thorium.Aggregator.Controllers;
using Thorium.Aggregator.Models;
using Thorium.Core.ApiGateway.ApiClient;
using Thorium.Mvc.Models;

namespace Thorium.Aggregator.AuthorizationModule.Controllers
{
    [Route("v{version:apiVersion}/authorization")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class AuthorizationAdminController : AggregatorBaseController
    {
        private readonly IAuthorizationAdminService _authorizationAdminService;

        public AuthorizationAdminController(
            IAuthorizationService authorizationService, 
            UserManager<ApplicationUser> userManager, 
            IAuthorizationAdminService authorizationAdminService, 
            ILogger logger, 
            ApiClientFactory apiClientFactory) : base(authorizationService,  userManager,apiClientFactory, logger)
        {
            _authorizationAdminService = authorizationAdminService;
        }

        [HttpGet("permissions")]
        [ProducesResponseType(typeof(PaginatedResponse<PermissionDto>), 200)]
        [ResourcePermission("permission", "search")]
        public async Task<IActionResult> GetPermissions(int roleId, int pageIndex, int pageSize)
        {
            var result = await _authorizationAdminService.GetPermissions(roleId, pageIndex, pageSize);
            return FromReply(result);
        }

        [HttpPost("permissions")]
        [ProducesResponseType(typeof(CreatedResponse<int>), 200)]
        [ResourcePermission("permission", "create")]
        public async Task<IActionResult> AddPermission([FromBody]PermissionDto permissionDto)
        {
            var result = await _authorizationAdminService.AddPermission(permissionDto);
            return FromReply(result);
        }

        [HttpPatch("permissions")]
        [ResourcePermission("permission", "create")]
        public async Task<IActionResult> UpdatePermission([FromBody]PermissionDto permissionDto)
        {
            var result = await _authorizationAdminService.UpdatePermission(permissionDto);
            return FromReply(result);
        }

        [HttpDelete("permissions/{permissionId}")]
        [ResourcePermission("permission", "delete")]
        public async Task<IActionResult> RemovePermission(int premissionId)
        {
            var result = await _authorizationAdminService.RemovePermission(premissionId);
            return FromReply(result);
        }
        
        [HttpPost("resources")]
        [ProducesResponseType(typeof(CreatedResponse<int>), 200)]
        [ResourcePermission("resource", "create")]
        public async Task<IActionResult> AddResource([FromBody]ResourceDto resourceDto)
        {
            var result = await _authorizationAdminService.AddResource(resourceDto);
            return FromReply(result);
        }

        [HttpPatch("resources")]
        [ResourcePermission("resource", "create")]
        public async Task<IActionResult> UpdateResource([FromBody]ResourceDto resourceDto)
        {
            var result = await _authorizationAdminService.UpdateResource(resourceDto);
            return FromReply(result);
        }
        
        [HttpDelete("resources/{resourceId}")]
        [ResourcePermission("resource", "delete")]
        public async Task<IActionResult> RemoveResource(int resourceId)
        {
            var result = await _authorizationAdminService.RemoveResource(resourceId);
            return FromReply(result);
        }

        [HttpGet("resources")]
        [ResourcePermission("resource", "search")]
        public async Task<IActionResult> GetAllResources(int pageIndex, int pageSize)
        {
            var result = await _authorizationAdminService.GetAllResources( pageIndex, pageSize);
            return FromReply(result);
        }

        [HttpGet("resources/{resourceId}/details")]
        [ResourcePermission("resource-detail", "search")]
        public async Task<IActionResult> GetResourceDetails(int resourceId, int pageIndex, int pageSize)
        {
            var result = await _authorizationAdminService.GetResourceDetails(resourceId, pageIndex, pageSize);
            return FromReply(result);
        }
        
        [HttpGet("resources/{resourceId}")]
        [ResourcePermission("resource-detail", "read")]
        public async Task<IActionResult> GetResource(int resourceId)
        {
            var result = await _authorizationAdminService.GetResource(resourceId);
            return FromReply(result);
        }

        [HttpPost("resources/{resourceId}")]
        [ResourcePermission("resource-detail", "create")]
        public async Task<IActionResult> AddResourceDetail(int resourceId, [FromBody]ResourceDetailDto resourceDetailDto)
        {
            var result = await _authorizationAdminService.AddResourceDetail(resourceDetailDto);
            return FromReply(result);
        }

        [HttpPatch("resources/{resourceId}")]
        [ResourcePermission("resource-detail", "create")]
        public async Task<IActionResult> UpdateResourceDetail([FromBody]ResourceDetailDto resourceDetailDto)
        {
            var result = await _authorizationAdminService.UpdateResourceDetail(resourceDetailDto);
            return FromReply(result);
        }

        [HttpDelete("resources/{resourceId}/{resourceDetailId}")]
        [ResourcePermission("resource-detail", "delete")]
        public async Task<IActionResult> RemoveResourceDetail(int resourceId, int resourceDetailId)
        {
            var result = await _authorizationAdminService.RemoveResourceDetail( resourceDetailId);
            return FromReply(result);
        }

        [HttpPost("roles")]
        [ResourcePermission("role", "create")]
        public async Task<IActionResult> AddRole([FromBody]RoleDto roleDto)
        {
            
            var result = await _authorizationAdminService.AddRole(roleDto);
            return FromReply(result);
        }

        [HttpPatch("roles")]
        [ResourcePermission("role", "create")]
        public async Task<IActionResult> UpdateRole([FromBody]RoleDto roleDto)
        {
            var result = await _authorizationAdminService.UpdateRole(roleDto);
            return FromReply(result);
        }
        
        [HttpDelete("roles/{roleId}")]
        [ResourcePermission("role", "delete")]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            var result = await _authorizationAdminService.RemoveRole(roleId);
            return FromReply(result);
        }
        
        [HttpGet("roles/{roleId}")]
        [ResourcePermission("role", "read")]
        public async Task<IActionResult> GetRole(int roleId)
        {
            var result = await _authorizationAdminService.GetRole(roleId);
           
            return FromReply(result);
        }

        [HttpGet("roles")]
        [ProducesResponseType(typeof(List<Role>), 200)]
        [ResourcePermission("role", "search")]
        public async Task<IActionResult> GetAllRoles(int pageIndex, int pageSize)
        {
            var result = await _authorizationAdminService.GetAllRoles( pageIndex, pageSize);
           
            return FromReply(result);
        }

        [HttpGet("{userId}/roles")]
        [ProducesResponseType(typeof(List<Role>), 200)]
        [ResourcePermission("user-role", "search")]
        public async Task<IActionResult> GetUserRoles([ResourceId]string userId)
        {
            var result = await _authorizationAdminService.GetUserRoles(userId);
            return FromReply(result);
        }

        [HttpPost("{userId}/roles")]
        [ResourcePermission("user-role", "create")]
        public async Task<IActionResult> AddUserToRole(string userId, [FromBody] int roleId)
        {
            await _authorizationAdminService.ClearUserRoles(userId);
            var result = await _authorizationAdminService.AddUserToRole(userId, roleId);
            return FromReply(result);
        }
        [HttpDelete("roles/{userId}/{roleId}")]
        [ResourcePermission("user-role", "delete")]
        public async Task<IActionResult> RemoveUserFromRole(string userId, int roleId)
        {
            var result = await _authorizationAdminService.RemoveUserFromRole(userId, roleId);
            return FromReply(result);
        }
    }
}