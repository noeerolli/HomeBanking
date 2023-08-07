using Microsoft.EntityFrameworkCore;

namespace HomeBanking.Models
{
    public class HomeBankingContext : DbContext  //hereda de DbContext - representa un sesión con la base de datos, se puede usar para consultar y guardar instancias de sus entidades
    {
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) {}  //constructor base

        public DbSet<Client> Clients  { get; set; }  //agregamos la colleción de tipo Client - representa una tabla en la base de datos

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<ClientLoan> ClientLoans { get; set; }

        public DbSet<Card> Cards { get; set; }

    }
}
