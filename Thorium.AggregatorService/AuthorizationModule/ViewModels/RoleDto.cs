using System.Collections.Generic;

namespace Thorium.Aggregator.AuthorizationModule.ViewModels
{
    public class RoleDto
    {
        public int? ParentRoleId { get; set; }
        public string Description { get; set; }
        public int NamespaceId { get; set; }
        public int Id { get; set; }
        public List<PermissionDto> RolePermissions { get; set; }
    }
}