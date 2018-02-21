using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace OAuthServer.Configurations
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("socialnetwork", "Social Network")
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "socialnetwork",
                    ClientSecrets = new [] { new IdentityServer4.Models.Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new[] { "socialnetwork" }
                }
            };
        }

        public static List<TestUser> Users()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "username",
                    Password = "password"
                }
            };
            
        }

    }
}
