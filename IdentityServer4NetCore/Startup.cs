using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4NetCore
{
    class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IClientStore, CustomClientStore>();
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryApiResources(GetAllResources());

        }

        public IEnumerable<ApiResource> GetAllResources()
        {
            return new[]
            {
                // Add a resource for some set of APIs that we may be protecting
                // Note that the constructor will automatically create an allowed scope with
                // name and claims equal to the resource's name and claims. If the resource
                // has different scopes/levels of access, the scopes property can be set to
                // list specific scopes included in this resource, instead.
                new ApiResource(
                    "myAPIs",                                       // Api resource name
                    "My API Set #1",                                // Display name
                    new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, "office" }) // Claims to be included in access token
                };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }

    public class CustomClientStore : IClientStore
    {
        public static IEnumerable<Client> AllClients { get; } = new[]
        {
        new Client
        {
            ClientId = "myClient",
            ClientName = "My Custom Client",
            AccessTokenLifetime = 60 * 60 * 24,
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            RequireClientSecret = false,
            AllowedScopes =
            {
                "myAPIs"
            }
        }
    };

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(AllClients.FirstOrDefault(c => c.ClientId == clientId));
        }
    }
}
