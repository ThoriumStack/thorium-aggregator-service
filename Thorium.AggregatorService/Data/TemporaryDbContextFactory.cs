using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Thorium.Aggregator.Data
{
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine("config","appsettings.json"))
                .Build();

            var builder = new DbContextOptionsBuilder<UsersDbContext>();
            var connectionString = configuration.GetConnectionString("UsersDb");

            builder.UseNpgsql(connectionString);

            return new UsersDbContext(builder.Options);
        }
    }
}