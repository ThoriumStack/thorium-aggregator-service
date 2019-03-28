using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Thorium.Aggregator.AuthorizationModule.Models;
using Thorium.Core.MicroServices.ConfigurationModels;
using Thorium.Core.Model;

namespace Thorium.Aggregator.AuthorizationModule
{
    public class AuthorizationService : IAuthorizationService
    {
        private DbSettings _authContextSettings;
        
        public AuthorizationService(List<DbSettings> dbSettings)
        {
            _authContextSettings = dbSettings.Find(c => c.Name == "AuthorizationDb");
        }
        
        public ReplyBase IsAuthorized(string userId, string resourceKey, string resourceDetailKey, string specificResourceId=null)
        {
            using (var context = new AuthorizationDbContext(_authContextSettings.ConnectionString))
            {
                var userRoles = context.UserRoles.Where(c => c.UserId == userId).ToList();
                if (!userRoles.Any())
                {
                    return ReplyBase.Unauthorized();
                }

                var resource = context.Resources.FirstOrDefault(c => c.Key == resourceKey);

                if (resource == null)
                {
                    resource = new Resource
                    {
                        Description = resourceKey,
                        Key = resourceKey
                    };
                    context.Resources.Add(resource);
                    context.SaveChanges();
                    //return ReplyBase.Unauthorized("Invalid resource key");
                }

                var resourceDetail =
                    context.ResourceDetails.FirstOrDefault(c =>
                        c.ResourceId == resource.Id && c.Key == resourceDetailKey);

                if (resourceDetail == null)
                {
                    resourceDetail = new ResourceDetail
                    {
                        Description = resourceDetailKey,
                        Key = resourceDetailKey,
                        ResourceId = resource.Id
                    };
                    
                    context.ResourceDetails.Add(resourceDetail);
                    context.SaveChanges();

                    
                    //return ReplyBase.Unauthorized("Invalid resource detail id");
                }

                var roles = context.Roles.Where(c => userRoles.Select(u => u.RoleId).Contains(c.Id)).ToList();
                if (roles.Any(c => c.Description == "Super User"))
                {
                    return ReplyBase.Success();
                }

                var roleIds = roles.Select(c => c.Id).ToList();

                var permissions = context.Permissions
                    .Where(c => roleIds.Contains(c.RoleId) && c.ResourceDetailId == resourceDetail.Id).ToList();
                
                if (!permissions.Any())
                {
                    return ReplyBase.Unauthorized();
                }

                if (permissions.Any(c =>
                    c.ResourceDetailId == resourceDetail.Id && c.PermissionType == PermissionType.Deny))
                {
                    return ReplyBase.Unauthorized();
                }

                var allowedPermission = permissions.FirstOrDefault(c =>
                    c.ResourceDetailId == resourceDetail.Id && c.PermissionType == PermissionType.Allow);
                if (allowedPermission != null)
                {
                    if (allowedPermission.OwnerScoped && specificResourceId != null)
                    {
                        var allowed = context.ResourceOwners.Any(c =>
                            c.ResourceId == resource.Id && c.SpecificResourceId == specificResourceId &&
                            c.UserId == userId);
                        if (!allowed)
                        {
                            return ReplyBase.Unauthorized();
                        }

                    }
                    return ReplyBase.Success();
                }

            }
            return ReplyBase.Unauthorized("Unknown resource check");
        }

        
        
        

        public Role GetOrCreateSuperUser(string userId)
        {
            using (var context = new AuthorizationDbContext(_authContextSettings.ConnectionString))
            {
                var superUser = context.Roles.FirstOrDefault(c=>c.Description == "Super User");
                if (superUser == null)
                {
                    superUser = new Role
                    {
                        Description = "Super User",

                    };
                    context.Roles.Add(superUser);
                    context.SaveChanges();
                }

                context.UserRoles.Add(new UserRole()
                {
                    RoleId = superUser.Id,
                    UserId = userId
                });
                context.SaveChanges();
                
                return superUser;
                
            }
        }

        public object GetUserPermissions(string userId)
        {
            using (var context = new AuthorizationDbContext(_authContextSettings.ConnectionString))
            {
                
                var userRoles = context.UserRoles
                    .Where(c => c.UserId == userId)
                    .Select(p => p.RoleId)
                    .ToList();
                
                var roleDescription = context.Roles.FirstOrDefault(c=>c.Id == userRoles.FirstOrDefault());
                if (roleDescription == null)
                {
                    return "";
                }
                if (roleDescription?.Description == "Super User")
                {
                    var allPerms = context.ResourceDetails.Include(c => c.Resource).ToList(); //.Include(c=>c.ResourceDetail.Resource).ToList();
                    return allPerms.Select(c => $"{c.Resource.Key}_{c.Key}");
                }
                // 
                var perms = context.Permissions
                    .Include(c=>c.ResourceDetail)
                    .Include(c=>c.ResourceDetail.Resource)
                    .Where(c => userRoles.Contains(c.RoleId))
                    .Select(c => $"{c.ResourceDetail.Resource.Key}_{c.ResourceDetail.Key}")
                    .ToList();

                return perms;
            }
        }

        public void AssignResourceToOwner(string resourceKey, string userId, string specificResourceId)
        {
            using (var context = new AuthorizationDbContext(_authContextSettings.ConnectionString))
            {
                var resource = context.Resources.FirstOrDefault(c => c.Key == resourceKey);

                if (resource == null)
                {
                    throw new Exception($"Unable to assign resource to user. Unknown resource key {resourceKey}");
                }

                if (UserAlreadyOwnsResource(context, resource, userId, specificResourceId))
                {
                    return;
                }
                
                context.ResourceOwners.Add(new ResourceOwner
                {
                    ResourceId = resource.Id,
                    UserId = userId,
                    SpecificResourceId = specificResourceId
                });
                context.SaveChanges();
            }
        }

        private bool UserAlreadyOwnsResource(AuthorizationDbContext dbContext, Resource resource, string userId, string specificResourceId)
        {
            return dbContext.ResourceOwners.Any(c =>
                c.ResourceId == resource.Id && c.UserId == userId && c.SpecificResourceId == specificResourceId);
        }

        public void AssignWebsiteUser(string userId)
        {
            using (var context = new AuthorizationDbContext(_authContextSettings.ConnectionString))
            {
                var webUser = context.Roles.FirstOrDefault(c=>c.Description == "Website User");
                if (webUser == null)
                {
                    webUser = new Role
                    {
                        Description = "Website User",

                    };
                    context.Roles.Add(webUser);
                    context.SaveChanges();
                }

                context.UserRoles.Add(new UserRole()
                {
                    RoleId = webUser.Id,
                    UserId = userId
                });
                context.SaveChanges();
                
                //return webUser;
                
            }
        }

        public string CurrentUserId { get; set; }
        public string CurrentContext { get; set; }
        public int CurrentTimeZoneOffset { get; set; }
    }
}