using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Thorium.Aggregator.AuthorizationModule.Filters;
using Thorium.Aggregator.AuthorizationModule.Models;

namespace Thorium.Aggregator.AuthorizationModule
{
    public class DefaultRoleSeeder
    {
        private AuthorizationDbContext _authDbContext;

        public DefaultRoleSeeder(AuthorizationDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public void SeedAuthorizationData()
        {
            SeedResources();
            CreateSuperUserRole();
            CreateWebsiteUserRole();
            
        }

        
        /// <summary>
        /// find all declared resources and add them to the db.
        /// </summary>
        private void SeedResources()
        {
            var assembly = Assembly.GetEntryAssembly();
           var attribs =  assembly.GetTypes()
                .SelectMany(type => type.GetMembers())
                .Union(assembly.GetTypes())
                .Where(type => Attribute.IsDefined(type, typeof(ResourcePermissionAttribute)));
            foreach (var memberInfo in attribs)
            {
                var permResource = memberInfo.GetCustomAttribute<ResourcePermissionAttribute>();

                var resource = _authDbContext.Resources.FirstOrDefault(c => c.Key == permResource.ResourceKey);
                if (resource == null)
                {
                    resource = new Resource
                    {
                        Key = permResource.ResourceKey,
                        Description = permResource.ResourceKey
                    };
                    _authDbContext.Resources.Add(resource);
                }

                var resourceDetail = _authDbContext.ResourceDetails.FirstOrDefault(c =>
                    c.ResourceId == resource.Id && c.Key == permResource.ResourceDetailKey);

                if (resourceDetail == null)
                {
                    resourceDetail = new ResourceDetail()
                    {
                        Key = permResource.ResourceDetailKey,
                        Description = permResource.ResourceDetailKey,
                        ResourceId = resource.Id
                    };
                    _authDbContext.ResourceDetails.Add(resourceDetail);
                }

                _authDbContext.SaveChanges();
            }

           
            
            

        }

        private void CreateWebsiteUserRole()
        {
            var webUser = _authDbContext.Roles.FirstOrDefault(c => c.Description == "Website User");
            if (webUser == null)
            {
                webUser = new Role
                {
                    Description = "Website User",
                    RolePermissions = GetWebsiteUserPermissions()
                };
                _authDbContext.Roles.Add(webUser);
                _authDbContext.SaveChanges();
            }
            
        }

        private List<Permission> GetWebsiteUserPermissions()
        {
            var result = new List<Permission>
            {

                CreatePermission("user", "read", true),
            };
            
            return result.Where(c=> c!= null).ToList();
        }

        private Permission CreatePermission(string resourceKey, string resourceDetailKey, bool ownerScoped)
        {
            var resource = _authDbContext.Resources.FirstOrDefault(c => c.Key == resourceKey);
            var resourceDetail =
                _authDbContext.ResourceDetails.FirstOrDefault(c =>
                    c.ResourceId == resource.Id && c.Key == resourceDetailKey);

            if (resourceDetail == null)
            {
                return null;
            }
            
            return new Permission
            {
                OwnerScoped = ownerScoped,
                PermissionType = PermissionType.Allow,
                ResourceDetailId = resourceDetail.Id
            };
        }

        private void CreateSuperUserRole()
        {
            var superUserRole = _authDbContext.Roles.FirstOrDefault(c => c.Description == "Super User");

            if (superUserRole == null)
            {
                superUserRole = new Role
                {
                    Description = "Super User",
                };
                _authDbContext.Roles.Add(superUserRole);
                _authDbContext.SaveChanges();
            }
        }
    }
}