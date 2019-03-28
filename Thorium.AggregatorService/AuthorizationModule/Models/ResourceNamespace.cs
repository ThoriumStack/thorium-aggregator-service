using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("ResourceNamespaces")]
    public class ResourceNamespace
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int NamespaceId { get; set; }
        
        public virtual Resource Resource { get; set; }
        public virtual AuthorizationNamespace AuthorizationNamespace { get; set; }
    }
}