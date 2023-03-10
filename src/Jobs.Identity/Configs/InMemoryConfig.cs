using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace Jobs.Identity.Config;

public static class InMemoryConfig
{
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Address(),
        new IdentityResource("roles", "User role(s)", new List<string>
        {
            "role"
        })
    };

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new ApiScope("jobsapi.scope", "Jobs Api")
    };

    public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
    {
        new ApiResource("jobsapi", "Jobs Api")
        {
            Scopes = { "jobsapi.scope" },
            UserClaims =
            {
                "role"
            }
        }
    };

    public static IEnumerable<Client> Clients => new List<Client>
    {
        new Client
        {
            ClientId = "first-client",
            ClientSecrets = new [] { new Secret("secret".Sha512()) },
            // Provides the information about the flow we are going to use to deliver the token to the client.
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                "jobsapi.scope"
            }
        },
        new Client
        {
            ClientName = "MvcClient",
            ClientId = "mvc-client",
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris =
            {
                "https://localhost:7002/signin-oidc"
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Address,
                "roles",
                "jobsapi.scope"
            },
            ClientSecrets =
            {
                new Secret("mvc-client-secret".Sha512())
            },
            RequirePkce = true,
            RequireConsent = true,
            PostLogoutRedirectUris =
            {
                "https://localhost:7002/signout-callback-oidc"
            }
        }
    };

    public static List<TestUser> TestUsers => new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "Bob",
            Password = "password_bob",
            Claims = new List<Claim>
            {
                new Claim("given_name", "Bob"),
                new Claim("family_name", "Bobsen"),
                new Claim("address", "123 Earth"),
                new Claim("role", "Admin")

            }
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "Steve",
            Password = "password_steve",
            Claims = new List<Claim>
            {
                new Claim("given_name", "Steve"),
                new Claim("family_name", "Stevesen"),
                new Claim("address", "321 Jupiter"),
                new Claim("role", "Editor")
            }
        }
    };
}
