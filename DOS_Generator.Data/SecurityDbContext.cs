using DOS_Generator.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DOS_Generator.Data
{
    public class SecurityDbContext : DbContext
    {
        public SecurityDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Declaration> Declarations { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MailServer> MailServers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
    }
}