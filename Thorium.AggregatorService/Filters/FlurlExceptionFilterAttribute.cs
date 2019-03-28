using System.Net;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Thorium.Mvc.Models;

namespace Thorium.Aggregator.Filters
{
    /// <summary>
    /// Convert  responses to front end responses
    /// </summary>
    public class FlurlExceptionFilterAttribute : ExceptionFilterAttribute
    {
       
        public FlurlExceptionFilterAttribute()
        {
            
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is FlurlHttpException ex)
            {
                var apiResponse = ex.GetResponseJsonAsync<ApiResponse>().GetAwaiter().GetResult();
                
                if (apiResponse == null)
                {
                    apiResponse = new ApiResponse("Service unavailable: " + ex.Call.FlurlRequest.Url);
                }
                
                var result =
                    new ObjectResult(apiResponse)
                    {
                        StatusCode = (int)(ex.Call.Response?.StatusCode ?? HttpStatusCode.ServiceUnavailable) 
                    };

                context.Result = result;
                context.ExceptionHandled = true;
            }
        }
    }
}