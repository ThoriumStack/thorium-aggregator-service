using System.Threading.Tasks;
using Thorium.Aggregator.AuthorizationModule.ViewModels;
using Thorium.Core.Model;
using Thorium.Core.Model.Abstractions;

namespace Thorium.Aggregator.AuthorizationModule
{
    public interface IAuthorizationAdminService : IServiceBase
    {
        Task<IdReply> AddAuthorizationNamespace(AuthorizationNamespaceDto authorizationNamespaceDto);
        Task<ReplyBase> UpdateAuthorizationNamespace(AuthorizationNamespaceDto authorizationNamespaceDto);
        Task<ReplyBase> RemoveAuthorizationNamespace(int id);
        Task<PaginatedListReply<PermissionDto>> GetPermissions(int roleId, int pageIndex, int pageSize);

        Task<IdReply> AddPermission(
            PermissionDto permissionDto);

        Task<ReplyBase> UpdatePermission(PermissionDto permissionDto);
        Task<ReplyBase> RemovePermission(int id);
        Task<IdReply> AddResource(ResourceDto resourceDto);
        Task<ReplyBase> UpdateResource(ResourceDto resourceDto);
        Task<ReplyBase> RemoveResource(int id);
        Task<PaginatedListReply<ResourceDto>> GetAllResources(int pageIndex, int pageSize);

        Task<PaginatedListReply<ResourceDetailDto>> GetResourceDetails(int resourceId, int pageIndex,
            int pageSize);

        Task<IdReply> AddResourceDetail(ResourceDetailDto resourceDetailDto);
        Task<ReplyBase> UpdateResourceDetail(ResourceDetailDto resourceDetailDto);
        Task<ReplyBase> RemoveResourceDetail(int id);
        Task<IdReply> AddRole(RoleDto roleDto);
        Task<ReplyBase> UpdateRole(RoleDto roleDto);
        Task<ReplyBase> RemoveRole(int id);
        Task<PaginatedListReply<RoleDto>> GetAllRoles(int pageIndex, int pageSize);
        Task<ListReply<UserRoleDto>> GetUserRoles(string userId);
        Task<IdReply> AddUserToRole(string userId, int roleId);
        Task<ReplyBase> RemoveUserFromRole(string userId, int roleId);
        Task ClearUserRoles(string userId);
        Task<SingleValueReply<ResourceDto>> GetResource(int resourceId);
        Task<SingleValueReply<RoleDto>> GetRole(int roleId);
    }
}