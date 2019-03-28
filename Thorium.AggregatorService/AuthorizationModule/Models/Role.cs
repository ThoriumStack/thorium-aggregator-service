using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public int? ParentRoleId { get; set; }
        public string Description { get; set; }
        public int NamespaceId { get; set; }
        
        public List<Permission> RolePermissions { get; set; }
    }
}