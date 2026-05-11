using Microsoft.EntityFrameworkCore;
using SecureVault.Api.Models;

namespace SecureVault.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Secret> Secrets { get; set; }
    }
}
