using HomeBanking.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;




namespace HomeBanking.Repositories
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository //hereda de repository base e implementa IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }


        //hereda todos los métodos, pero implemento los metodos establecidos en la interfaz
        public Client FindById(long id)
        {
            return FindByCondition(client => client.Id == id)   //retorna un objeto de tipo cliente
                .Include(client => client.Accounts)          //ademas de traer cliente, con include se agrega otra query para q traiga las cuentas asociadas
                .Include(client => client.ClientLoans)
                    .ThenInclude(cl => cl.Loan)
                .Include(client => client.Cards)
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return FindAll()
                .Include(client => client.Accounts)
                .Include(client => client.ClientLoans)
                    .ThenInclude(cl => cl.Loan)
                .Include(client => client.Cards)
                .ToList();
        }

       

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }

        public Client FindByEmail(string email)
        {
            return FindByCondition(client => client.Email.ToUpper() == email.ToUpper())
            .Include(client => client.Accounts)
            .Include(client => client.ClientLoans)
                .ThenInclude(cl => cl.Loan)
            .Include(client => client.Cards)
            .FirstOrDefault();
        }

    }
}
