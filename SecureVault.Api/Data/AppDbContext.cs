using Microsoft.EntityFrameworkCore;
using SecureVault.Api.Models;

namespace SecureVault.Api.Data
{
    /// <summary>
    /// Classe de contexto do Entity Framework Core. 
    /// Atua como a ponte de comunicação entre a API e o banco de dados (Nesse caso o postgresql).
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Representa a tabela "Secrets" no banco de dados.
        /// </summary>
        public DbSet<Secret> Secrets { get; set; }
    }
}
