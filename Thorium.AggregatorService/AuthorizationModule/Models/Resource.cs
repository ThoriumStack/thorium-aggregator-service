using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("Resources")]
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public List<ResourceDetail> ResourceDetails { get; set; }
    }
}