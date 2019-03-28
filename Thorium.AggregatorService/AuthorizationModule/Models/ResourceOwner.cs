using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("ResourceOwners")]
    public class ResourceOwner
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string UserId { get; set; }
        public string SpecificResourceId { get; set; }
    }
}