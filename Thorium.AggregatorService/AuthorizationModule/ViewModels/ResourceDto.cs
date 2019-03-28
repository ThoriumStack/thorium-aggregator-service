using System.Collections.Generic;

namespace Thorium.Aggregator.AuthorizationModule.ViewModels
{
    public class ResourceDto
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public List<ResourceDetailDto> ResourceDetails { get; set; }
    }
}