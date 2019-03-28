using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("ResourceDetails")]
    public class ResourceDetail
    {
        
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}