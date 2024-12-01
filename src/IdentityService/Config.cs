using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("microserviceApp", "microService full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "postman",
                ClientName = "postman",
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword},
                ClientSecrets = new[] { new Secret("SoBigSecret".Sha256()) },
                RedirectUris = { "https://www.getpostman.com"},
                AllowedScopes = { "openid", "profile", "microserviceApp" }
            },
        };
}
