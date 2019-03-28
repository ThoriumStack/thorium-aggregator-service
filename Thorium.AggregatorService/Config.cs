using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using Thorium.Aggregator.ConfigurationModels;

namespace Thorium.Aggregator
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(), 
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("dataEventRecords")
                {
                    ApiSecrets =
                    {
                        new Secret("dataEventRecordsSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "dataeventrecords",
                            DisplayName = "Scope for the dataEventRecords ApiResource"
                        }
                    },
                    UserClaims = { "role", "admin", "user" }
                },
                
                
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(List<ClientSetting> config)
        {

            // client credentials client


            var backOfficeWebConfig = config.First(x => x.ClientId == "back_office_web");
            var clientWebConfig = config.First(x => x.ClientId == "customer_web");
            
            return new List<Client>
            {
                
                new Client
                {
                    
                    ClientName = "Back Office Web Client",
                    ClientId = "back_office_web",
                   // AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = backOfficeWebConfig.AccessTokenLifetime,
                    IdentityTokenLifetime = backOfficeWebConfig.IdentityTokenLifetime,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = backOfficeWebConfig.AllowAccessTokensViaBrowser,
                    
                    RedirectUris = backOfficeWebConfig.RedirectUris,
                    PostLogoutRedirectUris = backOfficeWebConfig.PostLogoutRedirectUris,
                    AllowedCorsOrigins = backOfficeWebConfig.AllowedCorsOrigins,
                    AllowedScopes = backOfficeWebConfig.AllowedScopes,
                    LogoUri = backOfficeWebConfig.LogoUri,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
                new Client
                {
                    ClientName = "CustomerWeb",
                    ClientId = "customer_web",
                    // AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = clientWebConfig.AccessTokenLifetime,
                    IdentityTokenLifetime = clientWebConfig.IdentityTokenLifetime,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = clientWebConfig.AllowAccessTokensViaBrowser,
                    
                    RedirectUris =clientWebConfig.RedirectUris,
                    PostLogoutRedirectUris = clientWebConfig.PostLogoutRedirectUris,
                    AllowedCorsOrigins = clientWebConfig.AllowedCorsOrigins,
                    AllowedScopes = clientWebConfig.AllowedScopes,
                    RequireConsent = false
                }
            };
        }
    }
}