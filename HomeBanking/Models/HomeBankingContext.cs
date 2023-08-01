using Microsoft.EntityFrameworkCore;

namespace HomeBanking.Models
{
    public class HomeBankingContext : DbContext  //contexto p la conexión a la base de datos
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) {}

        public DbSet<Client> Clients  { get; set; }  //agregamos la colleción de tipo Client

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<ClientLoan> ClientLoans { get; set; }

    }
}
