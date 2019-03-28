using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Thorium.Aggregator.AuthorizationModule;
using Thorium.Aggregator.ConfigurationModels;
using Thorium.Aggregator.Data;
using Thorium.Aggregator.Endpoints;
using Thorium.Aggregator.Services;
using Thorium.Core.ApiGateway.ApiClient;
using Thorium.Core.ApiGateway.ApiClient.Models;
using Thorium.Core.MicroServices;
using Thorium.Core.MicroServices.Abstractions;
using Thorium.Core.MicroServices.ConfigurationModels;
using Thorium.Core.MicroServices.LivenessChecks;

namespace Thorium.Aggregator
{
    public class AggregatorServiceStartup : IServiceStartup, ISeedable, ICustomLogging, ICanCheckLiveness
    {
        public void ConfigureService(ServiceConfiguration configuration)
        {
            InjectSettings(configuration);

            configuration.InjectAutoMapper();
            configuration.InjectServiceBase();
            configuration.Inject<IAuthorizationAdminService, AuthorizationAdminService>();
            configuration.Inject<IAuthorizationService, AuthorizationService>();
            configuration.Inject<IEmailSender, EmailSender>();
            configuration.Inject<ApiClientFactory>();
            
            InjectCustomerModule(configuration);
            
            InjectWebEndpoints(configuration);
        }

        private static void InjectSettings(ServiceConfiguration configuration)
        {
            configuration.AddConfiguration<WebServiceSettings>();
            configuration.AddConfiguration<RabbitMqSettings>();
            configuration.AddConfiguration<List<ServiceEndpointSettings>>();
            configuration.AddConfiguration<List<ClientSetting>>();
        }

        private static void InjectCustomerModule(ServiceConfiguration configuration)
        {
            // Repositories
            //configuration.Inject<IUserRepository, UserRepository>();
            
            // Services
            //configuration.Inject<IUserService, UserService>();
            //configuration.InjectAutoMapper();
        }

        private static void InjectWebEndpoints(ServiceConfiguration configuration)
        {
            configuration.AddServiceEndpoint<IdentityWebEndpoint>();
        }

        public void SeedData(List<DbSettings> databaseSettings, ILogger logger)
        {
//            var customerDbContext = databaseSettings.Find(c => c.Name == "CustomerDb");
//            var claimDbContext = new CustomerDbContext(customerDbContext.ConnectionString);
//
//            claimDbContext.Database.Migrate();
            
            var authContext = new AuthorizationDbContext(databaseSettings.Find(c=>c.Name == "AuthorizationDb").ConnectionString);
            authContext.Database.Migrate();
            var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
            
            optionsBuilder.UseNpgsql(databaseSettings.Find(c => c.Name == "UsersDb").ConnectionString);
            var userContext = new UsersDbContext(optionsBuilder.Options);
            userContext.Database.Migrate();
            
            var seeder = new DefaultRoleSeeder(authContext);
            seeder.SeedAuthorizationData();
        }

        public void ConfigureLogging(LoggerConfiguration logger)
        {
            logger.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose);
        }

        public void ConfigureLivenessChecks(LivenessCheckConfiguration config)
        {
          //  config.AddCheck<DatabaseLivenessCheck<UsersDbContext>>();
            config.AddCheck<DatabaseLivenessCheck<AuthorizationDbContext>>();
        }
    }
}