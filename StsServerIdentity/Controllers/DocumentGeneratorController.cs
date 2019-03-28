using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBucks.Core.ApiGateway.ApiClient;
using MyBucks.Core.ApiGateway.DocumentGeneratorClient.V1_0;
using MyBucks.Mvc.Tools;
using MyBucks.Mvc.Tools.Model;
using MyBucks.Services.DocumentGenerator.ServiceContract;
using StsServerIdentity.ConfigurationModels;
using StsServerIdentity.Filters;

namespace StsServerIdentity.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/templates/")]
    [Produces("application/json")]
    [FlurlExceptionFilter]
    public class TemplateAdminController : ApiController
    {
        private readonly List<ServiceEndpointSettings> _endpointSettings;

        public TemplateAdminController(List<ServiceEndpointSettings> endpointSettings) 
        {
            _endpointSettings = endpointSettings;
        }
        private MyBucksApiClient GetBucksApiClient()
        {
            return new MyBucksApiClient()
                .Configure(_endpointSettings.Find(c=>c.Name=="document-generator").Url, CurrentContext, CurrentUserId);
        }

        [ProducesResponseType(typeof(PaginatedResponse<TemplateDto>), 200)]
        [HttpGet]
        public async Task<IActionResult> GetTemplates([FromQuery]int pageIndex, [FromQuery]int pageSize)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.GetTemplates(pageIndex, pageSize);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TemplateDto), 200)]
        [HttpGet("{templateId}")]
        public async Task<IActionResult> GetTemplateById(int templateId)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.GetTemplate(templateId);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TemplateDto), 200)]
        [HttpGet("{templateKey}")]
        public async Task<IActionResult> GetTemplateByKey(string templateKey)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.GetTemplate(templateKey);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TemplateDto), 200)]
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody]TemplateDto template)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.CreateTemplate(template);
            return Ok(result);
        }
        
        [ProducesResponseType(typeof(TemplateDto), 200)]
        [HttpPatch]
        public async Task<IActionResult> UpdateTemplate([FromBody]TemplateDto template)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.UpdateTemplate(template);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveTemplateByKey(string templateKey)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.RemoveTemplate(templateKey);
            return Ok(result);
        }

        [HttpDelete("{templateId}")]
        public async Task<IActionResult> RemoveTemplateById(int templateId)
        {
            var cl = new TemplateAdminClient(GetBucksApiClient());
            var result = await cl.RemoveTemplate(templateId);
            return Ok(result);
        }
    }
}