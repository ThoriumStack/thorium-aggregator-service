using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ResourceDetailId { get; set; }
        public PermissionType PermissionType { get; set; }
        public bool OwnerScoped { get; set; }
        
        public virtual ResourceDetail ResourceDetail { get; set; }
        
        
    }

    public enum PermissionType
    {
        Allow = 1, 
        Deny = 2
    }
}