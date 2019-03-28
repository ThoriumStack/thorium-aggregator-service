using Microsoft.EntityFrameworkCore;
using Thorium.Aggregator.AuthorizationModule.Models;

namespace Thorium.Aggregator.AuthorizationModule
{
    public class AuthorizationDbContext : DbContext
    {
        private readonly string _connectionString;

        public AuthorizationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                _connectionString);
            base.OnConfiguring(optionsBuilder);
        }
        
        public virtual DbSet<AuthorizationNamespace> AuthorizationNamespaces { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<ResourceDetail> ResourceDetails { get; set; }
        public virtual DbSet<ResourceNamespace> ResourceNamespaces{ get; set; }
        public virtual DbSet<ResourceOwner> ResourceOwners { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
    }
}