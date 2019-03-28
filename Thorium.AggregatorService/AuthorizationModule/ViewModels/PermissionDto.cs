using Humanizer;
using Thorium.Aggregator.AuthorizationModule.Models;

namespace Thorium.Aggregator.AuthorizationModule.ViewModels
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ResourceDetailId { get; set; }
        public PermissionType PermissionType { get; set; }
        public string PermissionTypeDescription => PermissionType.ToString().Underscore();
        public bool OwnerScoped { get; set; }
        public string ResourceDetailDescription { get; set; }
        public string ResourceDescription { get; set; }
    }
}