using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspIdentityServer.IdentitySvr.Configurations
{
    public class Config
    {

        public static IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static List<TestUser> GetAllUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "javad",
                    Password = "123"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "321"
                },
            };
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("AspIdentityServer" , "Main Project ==> AspIdentityServer")
            };
        }

        public static IEnumerable<Client> GetAllClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "AspIdentityServer" }
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "AspIdentityServer" }
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = {"http://localhost:5003/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5003/signout-callback-oidc"},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        }
    }
}
