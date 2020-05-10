using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeShopTalk.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CoffeeShopTalk
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSignalR();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options =>
                    {
                        options.Authority = Configuration.GetValue<string>("Auth0:Domain");
                        options.ClientId = Configuration.GetValue<string>("Auth0:ClientId");
                        options.ClientSecret = Configuration.GetValue<string>("Auth0:ClientSecret");
                        options.ResponseType = OpenIdConnectResponseType.Code;

                        options.CallbackPath = new PathString("/callback");

                        options.ClaimsIssuer = "Auth0";

                        options.Events = new OpenIdConnectEvents()
                        {
                            OnRedirectToIdentityProviderForSignOut = (context) =>
                            {
                                var logoutUri = Configuration.GetValue<string>("Auth0:Domain") + "/v2/logout?client_id=" + Configuration.GetValue<string>("Auth0:ClientId");

                                var postLogoutUri = context.Properties.RedirectUri;

                                if (!string.IsNullOrEmpty(postLogoutUri))
                                {
                                    if (postLogoutUri.StartsWith("/"))
                                    {
                                        var request = context.Request;
                                        postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                    }
                                    logoutUri += "&returnTo=" + Uri.EscapeDataString(postLogoutUri);
                                }

                                context.Response.Redirect(logoutUri);
                                context.HandleResponse();

                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
