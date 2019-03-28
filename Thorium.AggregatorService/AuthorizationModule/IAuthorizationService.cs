using Thorium.Aggregator.AuthorizationModule.Models;
using Thorium.Core.Model;
using Thorium.Core.Model.Abstractions;

namespace Thorium.Aggregator.AuthorizationModule
{
    public interface IAuthorizationService : IServiceBase
    {
        ReplyBase IsAuthorized(string userId, string resourceKey, string resourceDetailKey, string specificResourceId=null);
        object GetUserPermissions(string userId);
        Role GetOrCreateSuperUser(string userId);
        void AssignResourceToOwner(string resourceKey, string userId, string specificResourceId);
        void AssignWebsiteUser(string userId);
    }
}