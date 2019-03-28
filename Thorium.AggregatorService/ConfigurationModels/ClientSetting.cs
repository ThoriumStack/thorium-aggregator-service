using System.Collections.Generic;

namespace Thorium.Aggregator.ConfigurationModels
{
    public class ClientSetting
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }    
        public bool AllowAccessTokensViaBrowser { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<string> PostLogoutRedirectUris { get; set; }
        public List<string> AllowedCorsOrigins { get; set; }
        public List<string> AllowedScopes { get; set; }
        public string LogoUri { get; set; }
    }
}