using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("UserRoles")]
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
        
        public virtual Role Role { get; set; }
        
    }
}