using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thorium.Aggregator.AuthorizationModule.Models
{
    [Table("AuthorizationNamespaces")]
    public class AuthorizationNamespace
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        
        public List<Role> Roles { get; set; }
    }
}