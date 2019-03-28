using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Thorium.Aggregator.AuthorizationModule.Models;
using Thorium.Aggregator.AuthorizationModule.ViewModels;
using Thorium.Core.DataTools;
using Thorium.Core.MicroServices.ConfigurationModels;
using Thorium.Core.Model;

namespace Thorium.Aggregator.AuthorizationModule
{
    public class AuthorizationAdminService : IAuthorizationAdminService
    {
        private DbSettings _authContextSettings;

        public AuthorizationAdminService(List<DbSettings> dbSettings)
        {
            _authContextSettings = dbSettings.Find(c => c.Name == "AuthorizationDb");
        }

        private AuthorizationDbContext GetContext()
        {
            return new AuthorizationDbContext(_authContextSettings.ConnectionString);
        }
        
        public async Task<IdReply> AddAuthorizationNamespace(AuthorizationNamespaceDto authorizationNamespaceDto)
        {
            using (var authContext = GetContext())
            {
                var ns = new AuthorizationNamespace
                {
                    Description = authorizationNamespaceDto.Description,
                    Key = authorizationNamespaceDto.Key
                };
                authContext.AuthorizationNamespaces.Add(ns);
                authContext.SaveChanges();
                return IdReply.Success(ns.Id);
            }
           
        }

        public async Task<ReplyBase> UpdateAuthorizationNamespace(AuthorizationNamespaceDto authorizationNamespaceDto)
        {
            using (var authContext = GetContext())
            {
                var existingNamespace = authContext.AuthorizationNamespaces.Find(authorizationNamespaceDto.Id);

                if (existingNamespace == null)
                {
                    return ReplyBase.NotFound($"Unable to find authorization namespace with id {authorizationNamespaceDto.Id}");
                }
                
                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<ReplyBase> RemoveAuthorizationNamespace(int id)
        {
            using (var authContext = GetContext())
            {
                var existingNamespace = authContext.AuthorizationNamespaces.Find(id);

                if (existingNamespace == null)
                {
                    return ReplyBase.NotFound($"Unable to find authorization namespace with id {id}");
                }
                authContext.SaveChanges();
                return IdReply.Success();
            }
        }

        public async Task<PaginatedListReply<PermissionDto>> GetPermissions(int roleId, int pageIndex, int pageSize)
        {
            using (var authContext = GetContext())
            {
                var resources = authContext.Permissions.Where(c=>c.RoleId == roleId);
                var paginatedResult = resources.Paginate<Permission, PermissionDto>(pageIndex, pageSize);
                return paginatedResult;
            }
        }

        public async Task<IdReply> AddPermission(
            PermissionDto permissionDto)
        {
            using (var authContext = GetContext())
            {
                var ns = new Permission
                {
                    OwnerScoped = permissionDto.OwnerScoped,
                    PermissionType = permissionDto.PermissionType,
                    RoleId = permissionDto.RoleId,
                    ResourceDetailId = permissionDto.ResourceDetailId
                };
                authContext.Permissions.Add(ns);
                authContext.SaveChanges();
                return IdReply.Success(ns.Id);
            }
        }

        public async Task<ReplyBase> UpdatePermission(PermissionDto permissionDto)
        {
            using (var authContext = GetContext())
            {
                var existingPermission = authContext.Permissions.Find(permissionDto.Id);
                if (existingPermission == null)
                {
                    return ReplyBase.NotFound($"Unable to find permission with id {permissionDto.Id}");
                }

                existingPermission.OwnerScoped = permissionDto.OwnerScoped;
                existingPermission.PermissionType = permissionDto.PermissionType;
                existingPermission.RoleId = permissionDto.RoleId;
                existingPermission.ResourceDetailId = permissionDto.ResourceDetailId;
                
                
                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<ReplyBase> RemovePermission(int id)
        {
            using (var authContext = GetContext())
            {
                var existingPermission = authContext.Permissions.Find(id);
                if (existingPermission == null)
                {
                    return ReplyBase.NotFound($"Unable to find permission with id {id}");
                }
                
                
                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<IdReply> AddResource(ResourceDto resourceDto)
        {
            using (var authContext = GetContext())
            {
                var ns = new Resource
                {
                    Description = resourceDto.Description,
                    Key = resourceDto.Key,
                    ResourceDetails = resourceDto.ResourceDetails.Select(c=> new ResourceDetail
                    {
                        Description = c.Description,
                        Key = c.Key,
                        
                    }).ToList()
                    
                    
                };
                authContext.Resources.Add(ns);
                authContext.SaveChanges();
                return IdReply.Success(ns.Id);
            }
        }

        public async Task<ReplyBase> UpdateResource(ResourceDto resourceDto)
        {
            using (var authContext = GetContext())
            {
                var existingResource = authContext.Resources.Find(resourceDto.Id);

                if (existingResource == null)
                {
                    return  ReplyBase.NotFound($"Resource with id {resourceDto.Id} not found");
                }

                existingResource.Description = resourceDto.Description;
                existingResource.Key = resourceDto.Key;

                var details = authContext.ResourceDetails.Where(c => c.ResourceId == existingResource.Id).ToList();
               

                var resourceDetailsToDelete =
                    details.Where(d => !resourceDto.ResourceDetails.Select(c => c.Id).Contains(d.Id)).ToList();
                
                authContext.ResourceDetails.RemoveRange(resourceDetailsToDelete); 
                
                foreach (var resourceDtoResourceDetail in resourceDto.ResourceDetails)
                {
                    if (resourceDtoResourceDetail.Id != 0)
                    {
                        var existingResourceDetail = authContext.ResourceDetails.Find(resourceDtoResourceDetail.Id);
                        existingResourceDetail.Description = resourceDtoResourceDetail.Description;
                        existingResourceDetail.Key = resourceDtoResourceDetail.Key;
                    }
                    else
                    {
                        resourceDtoResourceDetail.ResourceId = resourceDto.Id;
                        authContext.ResourceDetails.Add(new ResourceDetail
                        {
                            Description = resourceDtoResourceDetail.Description,
                            ResourceId = resourceDto.Id,
                            Key = resourceDtoResourceDetail.Key
                        });    
                    }
                    
                }

                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<ReplyBase> RemoveResource(int id)
        {
            using (var authContext = GetContext())
            {
                var existingResource = authContext.Resources.Find(id);

                if (existingResource == null)
                {
                    return  ReplyBase.NotFound($"Resource with id {id} not found");
                }
                authContext.Resources.Remove(existingResource);
                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<PaginatedListReply<ResourceDto>> GetAllResources(int pageIndex, int pageSize)
        {
            using (var authContext = GetContext())
            {
                var resources = authContext.Resources;
                var paginatedResult = resources.Paginate<Resource, ResourceDto>(pageIndex, pageSize);
                return paginatedResult;
            }
        }

        public async Task<PaginatedListReply<ResourceDetailDto>> GetResourceDetails(int resourceId, int pageIndex,
            int pageSize)
        {
            using (var authContext = GetContext())
            {
                var resourceDetails = authContext.ResourceDetails.Where(c => c.ResourceId == resourceId);
                var paginatedResult = resourceDetails.Paginate<ResourceDetail, ResourceDetailDto>( pageIndex, pageSize);
                return paginatedResult;
            }
        }

        public async Task<IdReply> AddResourceDetail(ResourceDetailDto resourceDetailDto)
        {
            using (var authContext = GetContext())
            {
                var ns = new ResourceDetail
                {
                    Description = resourceDetailDto.Description,
                    Key = resourceDetailDto.Key,
                    ResourceId = resourceDetailDto.ResourceId
                };
                authContext.ResourceDetails.Add(ns);
                authContext.SaveChanges();
                return IdReply.Success(ns.Id);
            }
        }

        public async Task<ReplyBase> UpdateResourceDetail(ResourceDetailDto resourceDetailDto)
        {
            using (var authContext = GetContext())
            {
                var existingResourceDetail = authContext.ResourceDetails.Find(resourceDetailDto.Id);

                if (existingResourceDetail == null)
                {
                    return ReplyBase.NotFound($"Unable to find resource with ID {resourceDetailDto.Id}");
                }


                existingResourceDetail.Description = resourceDetailDto.Description;
                existingResourceDetail.Key = resourceDetailDto.Key;
                existingResourceDetail.ResourceId = resourceDetailDto.ResourceId;
                
                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }

        public async Task<ReplyBase> RemoveResourceDetail(int id)
        {
            using (var authContext = GetContext())
            {
                
                var existingResourceDetail = authContext.ResourceDetails.Find(id);

                if (existingResourceDetail == null)
                {
                    return ReplyBase.NotFound($"Unable to find resource detail with ID {id}");
                }

                authContext.SaveChanges();
                return ReplyBase.Success();
            }
        }
        
        

        public async Task<IdReply> AddRole(RoleDto roleDto)
        {
            using (var authContext = GetContext())
            {
                var ns = new Role
                {
                    Description = roleDto.Description,
                    NamespaceId = roleDto.NamespaceId,
                    ParentRoleId = roleDto.ParentRoleId,
                    RolePermissions = roleDto.RolePermissions.Select(c=> new Permission
                    {
                        ResourceDetailId = c.ResourceDetailId,
                        OwnerScoped = c.OwnerScoped,
                        PermissionType = c.PermissionType
                        
                    }).ToList()
                };
                
                
                
                authContext.Roles.Add(ns);
                authContext.SaveChanges();
                return IdReply.Success(ns.Id);
            }
        }

        public async Task<ReplyBase> UpdateRole(RoleDto roleDto)
        {
            using (var authContext = GetContext())
            {
                var existingRole = authContext.Roles
                    .Include(c=>c.RolePermissions)
                    .FirstOrDefault(c=>c.Id == roleDto.Id);

                if (existingRole == null)
                {
                    return ReplyBase.NotFound($"Unable to find role with id {roleDto.Id}");
                }

                var existingPermissions = authContext.Permissions.Where(c => c.RoleId == roleDto.Id).ToList();

                var permissionsToDelete = existingPermissions
                    .Where(c => !roleDto.RolePermissions.Select(d => d.Id).Contains(c.Id)).ToList();
                
                authContext.Permissions.RemoveRange(permissionsToDelete);
                foreach (var roleDtoRolePermission in roleDto.RolePermissions)
                {
                    if (roleDtoRolePermission.Id == 0)
                    {
                        existingRole.RolePermissions.Add(new Permission
                        {
                            OwnerScoped = roleDtoRolePermission.OwnerScoped,
                            PermissionType = roleDtoRolePermission.PermissionType,
                            ResourceDetailId = roleDtoRolePermission.ResourceDetailId,
                            RoleId = roleDtoRolePermission.RoleId
                            
                            
                        });
                    }
                    else
                    {
                        var existingPerm = existingRole.RolePermissions.First(c => c.Id == roleDtoRolePermission.Id);
                        existingPerm.PermissionType = roleDtoRolePermission.PermissionType;
                        existingPerm.OwnerScoped = roleDtoRolePermission.OwnerScoped;
                        existingPerm.ResourceDetailId = roleDtoRolePermission.ResourceDetailId;
                    }
                }

                existingRole.Description = roleDto.Description;
                existingRole.NamespaceId = roleDto.NamespaceId;
                existingRole.ParentRoleId = roleDto.ParentRoleId;
                authContext.SaveChanges();

            };
                return ReplyBase.Success();
            
        }

        public async Task<ReplyBase> RemoveRole(int id)
        {
            using (var authContext = GetContext())
            {
                var existingRole = authContext.Roles.Find(id); 
                if (existingRole == null)
                {
                    return ReplyBase.NotFound($"Unable to find role with id {id}");
                }
                authContext.Roles.Remove(existingRole);
                authContext.SaveChanges();
            }
            return ReplyBase.Success();
        }

        public async Task<PaginatedListReply<RoleDto>> GetAllRoles(int pageIndex, int pageSize)
        {
            using (var authContext = GetContext())
            {
                var roles = authContext.Roles;
                var pagedRoles = roles.Paginate<Role, RoleDto>(pageIndex, pageSize);
                return pagedRoles;
            }
        }

        public async Task<ListReply<UserRoleDto>> GetUserRoles(string userId)
        {
            using (var authContext = GetContext())
            {
                var userRoles = authContext.UserRoles.Where(c => c.UserId == userId).ToList();
                var userRoleDtos = new ListReply<UserRoleDto>();
                userRoleDtos.ResultList = new List<UserRoleDto>();
                foreach (var userRole in userRoles)
                {
                    var role = authContext.Roles.Find(userRole.RoleId);
                    userRoleDtos.ResultList.Add(new UserRoleDto
                    {
                        Id = userRole.Id,
                        RoleId = userRole.RoleId,
                        UserId = userRole.UserId,
                        RoleDescription = role.Description
                    });
                }
                return userRoleDtos;
            }
        }

        public async Task<IdReply> AddUserToRole(string userId, int roleId)
        {
            using (var authContext = GetContext())
            {
                var newUserRole = new UserRole
                {
                    RoleId = roleId,
                    UserId = userId
                };
                
                authContext.UserRoles.Add(newUserRole);
                authContext.SaveChanges();
                return IdReply.Success(newUserRole.Id);
            }
        }

        public async Task<ReplyBase> RemoveUserFromRole(string userId, int roleId)
        {
            
            using (var authContext = GetContext())
            {
                var existingRole = authContext.UserRoles.FirstOrDefault(c=>c.RoleId == roleId && c.UserId == userId); 
                if (existingRole == null)
                {
                    return ReplyBase.NotFound($"Unable to find user role  for user '{userId}' with id {roleId}");
                }
                authContext.UserRoles.Remove(existingRole);
                authContext.SaveChanges();
            }
            return ReplyBase.Success();
        }

        public async Task ClearUserRoles(string userId)
        {
            using (var authContext = GetContext())
            {
                var userRoles = authContext.UserRoles.Where(c => c.UserId == userId);
                authContext.UserRoles.RemoveRange(userRoles);
                authContext.SaveChanges();

            }
        }

        public async Task<SingleValueReply<ResourceDto>> GetResource(int resourceId)
        {
            using (var authContext = GetContext())
            {
                var resource = authContext.Resources.Include(c=>c.ResourceDetails).FirstOrDefault(c=>c.Id == resourceId);
                if (resource == null)
                {
                    return new SingleValueReply<ResourceDto>
                    {
                        ReplyStatus = ReplyStatus.NotFound,
                        ReplyMessage = $"Unable to find resource with id {resourceId}"
                    };
                }

                return SingleValueReply<ResourceDto>.Success(new ResourceDto
                {
                    Key = resource.Key,
                    Description = resource.Description,
                    Id = resourceId,
                    ResourceDetails = resource.ResourceDetails.Select(c=> new ResourceDetailDto
                    {
                        Description = c.Description,
                        Key = c.Key,
                        Id = c.Id,
                        ResourceId = c.ResourceId
                    }).ToList()
                    
                });

            }
        }

        public async Task<SingleValueReply<RoleDto>> GetRole(int roleId)
        {
            using (var authContext = GetContext())
            {
                var role = authContext.Roles
                    .Include(d => d.RolePermissions)
                    .ThenInclude(c=>c.ResourceDetail)
                    .ThenInclude(c=>c.Resource)
                    .FirstOrDefault(c => c.Id == roleId);

                if (role == null)
                {
                    return new SingleValueReply<RoleDto>
                    {
                        ReplyStatus = ReplyStatus.NotFound,
                        ReplyMessage = $"Unable to find role with id {roleId}"
                    };
                }
                
                return new SingleValueReply<RoleDto>(new RoleDto
                {
                    Description = role.Description,
                    NamespaceId = role.NamespaceId,
                    Id = role.Id,
                    RolePermissions = role.RolePermissions.Select(c=> new PermissionDto
                    {
                        Id = c.Id,
                        RoleId = role.Id,
                        OwnerScoped = c.OwnerScoped,
                        PermissionType = c.PermissionType,
                        ResourceDetailId = c.ResourceDetailId,
                        ResourceDetailDescription = c.ResourceDetail.Description,
                        ResourceDescription = c.ResourceDetail.Resource.Description,
                        
                        
                    }).ToList()
                });
            }
        }


        public string CurrentUserId { get; set; }
        public string CurrentContext { get; set; }
        public int CurrentTimeZoneOffset { get; set; }
    }
}