using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Thorium.Aggregator.AuthorizationModule;
using Thorium.Aggregator.ConfigurationModels;
using Thorium.Aggregator.Data;
using Thorium.Aggregator.Models;
using Thorium.Aggregator.Resources;
using Thorium.Aggregator.Services;
using Thorium.Core.MicroServices;
using Thorium.Core.MicroServices.Restful;
using Thorium.Core.MicroServices.Restful.Infrastructure;

namespace Thorium.Aggregator.RestServices
{
    public class IdentityRestService : RestServiceBase
    {
        private readonly IHostingEnvironment _environment;

        public IdentityRestService(IConfiguration configuration, IHostingEnvironment environment) :
            base(ServiceStartup.GetConfigurationRoot(), new AggregatorServiceStartup())
        {
            _environment = environment;
        }

        public override void ConfigureCustomServices(IServiceCollection services)
        {
            var stsConfig = Configuration.GetSection("StsConfig");
            var redisConfig = Configuration.GetSection("RedisConfig");
            var clientConfig = Configuration.GetSection("ClientSetting").Get<List<ClientSetting>>();
            var openIdConnectConfig = Configuration.GetSection("OpenIdConnectSettings").Get<OpenIdConnectSettings>();
            var useLocalCertStore = Convert.ToBoolean(Configuration["UseLocalCertStore"]);
            var certificateThumbprint = Configuration["CertificateThumbprint"];

            X509Certificate2 cert;

          
            
            cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "aggregator.pfx"), "test");

            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("UsersDb")));

            services.AddDbContext<AuthorizationDbContext>();

            services.Configure<StsConfig>(Configuration.GetSection("StsConfig"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<List<ClientSetting>>(Configuration.GetSection("ClientSetting"));
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                
            });

            services.AddSingleton<LocService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddApiVersioning(o =>
            {
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = ApiVersion.Default;

            });
            IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "oidc";



            }).AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = openIdConnectConfig.SignInScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.Authority = openIdConnectConfig.Authority;
                options.RequireHttpsMetadata = false;

                options.ClientId = clientConfig.First(x => x.ClientId == "back_office_web").ClientId;

                options.SaveTokens = true;
                options.SignedOutRedirectUri = "http://localhost:8080";
            }).AddIdentityServerAuthentication(options =>
            {
                options.Authority = openIdConnectConfig.Authority;
                options.RequireHttpsMetadata = false;

                options.ApiName = "credit_life";
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromMinutes(10); // that's the default
                
                
            });
            
           

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.WithExposedHeaders();
                    builder.AllowCredentials();
                    builder.SetIsOriginAllowed(s =>
                        {
                            foreach (var item in clientConfig)
                            {
                                if (item.AllowedCorsOrigins.Any(x => x == s)) return true;
                            }

                            return false;
                        });

                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("de-CH"),
                            new CultureInfo("fr-CH"),
                            new CultureInfo("it-CH")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "de-CH", uiCulture: "de-CH");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    var providerQuery = new LocalizationQueryProvider
                    {
                        QureyParameterName = "ui_locales"
                    };

                    // Cookie is required for the logout, query parameters at not supported with the endsession endpoint
                    // Only works in the same domain
                    var providerCookie = new LocalizationCookieProvider
                    {
                        CookieName = "defaultLocale"
                    };
                    // options.RequestCultureProviders.Insert(0, providerCookie);
                    options.RequestCultureProviders.Insert(0, providerQuery);
                });

            services.AddMvc(options =>
                {
                    options.Filters.Add(new ActionFilterDispatcher(_container.GetAllInstances));
                    
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)

                .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddIdentityServer(options =>
                {
                    options.IssuerUri = stsConfig["IssuerUrl"];
                    options.PublicOrigin = options.IssuerUri;
                })
                .AddSigningCredential(cert)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionString = redisConfig["Url"];
                    options.Db = 1;
                })
                .AddRedisCaching(options =>
                {
                    options.RedisConnectionString = redisConfig["Url"];
                    options.KeyPrefix = "idsrv";
                })
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(clientConfig))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "HTTP API",
                    Version = "v1"
                });

                // UseFullTypeNameInSchemaIds replacement for .NET Core
                options.CustomSchemaIds(x => x.FullName);
            });
        }

        public override void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            
            app.UseStaticFiles();
            app.UseCors();

            
            app.UseIdentityServer();
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/v1/swagger.json", "API V1");
                });
        }
    }
}