using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Thorium.Aggregator.Filters;
using Thorium.Aggregator.Models;
using Thorium.Core.ApiGateway.ApiClient;
using Thorium.Mvc.Tools;
using IAuthorizationService = Thorium.Aggregator.AuthorizationModule.IAuthorizationService;

namespace Thorium.Aggregator.Controllers
{
    [ApiVersion("1.0")]
    [FlurlExceptionFilter]
    [Authorize]
    [ApiController]
    public abstract class AggregatorBaseController : ApiController
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiClientFactory _apiClientFactory;
        private readonly ILogger _logger;

        public AggregatorBaseController(
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager,
            ApiClientFactory apiClientFactory,
            ILogger logger) : base(authorizationService)
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
            _apiClientFactory = apiClientFactory;
            _logger = logger;

            //_interactionService = interactionService;
        }

      //  protected CustomerDto CurrentCustomer { get; private set; }
        protected bool IsInternalUser { get; set; }
        protected bool IsAnonymousCall { get; set; }


        private T DigFor<T>(IDictionary<string, object> parameters, string parameterName)
        {
            var classes = parameters.Where(c => c.Value.GetType().IsClass)
                .ToList();
            foreach (var classObject in classes)
            {
                var props = classObject.Value.GetType().GetProperties();
                var objectParm = props.FirstOrDefault(c => c.Name == parameterName);
                if (objectParm == null)
                {
                    return default(T);
                }

                return (T) objectParm.GetValue(classObject.Value);
            }

            return default(T);
        }

//        private CustomerClient GetCustomerClient()
//        {
//            return _apiClientFactory.GetClient<CustomerClient>(CurrentContext, CurrentUserId, CurrentTimeZoneOffset);
//
//        }

        protected override void OnActionExecutingCustom(ActionExecutingContext context)
        {
            CurrentUserId = "";


            try
            {
                CurrentUserId = context.HttpContext.User.GetSubjectId();
            }
            catch (Exception e)
            {
                // just try to get the sub id, dont crash if you fail
            }

            if (!string.IsNullOrWhiteSpace(CurrentUserId))
            {
//                var customerClient = GetCustomerClient();
//
//
//                var user = _userManager.Users.FirstOrDefault(c => c.Id == CurrentUserId);
//                if (user == null)
//                {
//                    return;
//                }
//
//                IsInternalUser = user.IsInternal;
//                if (IsInternalUser)
//                {
//                    return;
//                }
//
//                try
//                {
//                    CurrentCustomer = customerClient.GetCustomerByUserId(CurrentUserId).GetAwaiter().GetResult();
//                }
//                catch (Exception e)
//                {
//                    _logger.Fatal(e, "Failed to get customer in controller.");
//                }
            }
            else
            {
                IsAnonymousCall = true;
            }
        }


        protected async Task AssignResourceOwner(string resourceKey, string resourceId)
        {
            if (IsAnonymousCall)
            {
                return;
            }

            if (!IsInternalUser)
            {
                _authorizationService.AssignResourceToOwner(resourceKey, CurrentUserId, resourceId);
            }
//            else if (!string.IsNullOrWhiteSpace(CurrentCustomer.UserId))
//            {
//                _authorizationService.AssignResourceToOwner(resourceKey, CurrentCustomer.UserId, resourceId);
//            }
        }

//        protected async Task AssignCustomerResourceOwner(string resourceKey, string resourceId, string resultCustomerId)
//        {
//            if (IsAnonymousCall)
//            {
//                return;
//            }
//        
//            int.TryParse(resultCustomerId, out var customerIdInt);
//            if (customerIdInt != 0)
//            {
//                CurrentCustomer = GetCustomerClient().GetCustomer(customerIdInt).GetAwaiter().GetResult();
//            }
//
//            await AssignResourceOwner(resourceKey, resourceId);
//        }
        
//        protected async Task AssignCustomerResourceOwner(string resourceKey, int resourceId, string resultCustomerId)
//        {
//            await AssignCustomerResourceOwner(resourceKey, resourceId.ToString(), resultCustomerId);
//        }
    }
}