using System;
using System.Linq;
using System.Net;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Thorium.Aggregator.AuthorizationModule.Attributes;
using Thorium.Core.MicroServices.Restful.Infrastructure;

namespace Thorium.Aggregator.AuthorizationModule.Filters
{
    public class ResourcePermissionAttribute : Attribute
    {
        public string ResourceKey { get; }
        public string ResourceDetailKey { get; }


        public ResourcePermissionAttribute(string resourceKey, string resourceDetailKey)
        {
            ResourceKey = resourceKey;
            ResourceDetailKey = resourceDetailKey;
        }
    }
    
    public class ResourcePermissionActionFilter : IActionFilter<ResourcePermissionAttribute>
    {
        private readonly IAuthorizationService _authorizationService;

        public ResourcePermissionActionFilter(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
      

        public void OnActionExecuting(ResourcePermissionAttribute attribute, ActionExecutingContext context)
        {
            if (context == null)
            {
                return;
            }
            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)) return;
            var method = controllerActionDescriptor.MethodInfo;


            string resourceIdField = null;
            var parms = method.GetParameters();
            foreach (var parameterInfo in parms)
            {
                var resourceId = parameterInfo.GetCustomAttributes(typeof(ResourceIdAttribute), false)
                    .FirstOrDefault();

                if (resourceId != null)
                {
                    resourceIdField = parameterInfo.Name;
                    
                }
            }

            string resourceIdValue = null;
            
            
            
            if (resourceIdField != null && context.RouteData.Values.ContainsKey(resourceIdField))
            {
                resourceIdValue = context.RouteData.Values[resourceIdField].ToString();
            }

            if (resourceIdField != null && resourceIdValue == null && context.ActionArguments.ContainsKey(resourceIdField))
            {
                resourceIdValue = context.ActionArguments[resourceIdField].ToString();
            }

            var userId = context.HttpContext.User.Identity.GetSubjectId();

            var authorized =
                _authorizationService.IsAuthorized(userId, attribute.ResourceKey, attribute.ResourceDetailKey, resourceIdValue);

            if (!authorized.IsSuccessful())
            {
                context.Result = new StatusCodeResult((int) HttpStatusCode.Forbidden);
            }
        }
    }
}