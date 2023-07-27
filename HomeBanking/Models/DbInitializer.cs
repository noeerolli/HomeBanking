using System;
using System.Linq;
using System.Runtime.Serialization;

namespace HomeBanking.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context) 
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client  {
                        FirstName = "Noe",
                        LastName = "Rolli",
                        Email = "blabla@gmail.com",
                        Password = "12345"
                    },
                    new Client  {
                        FirstName = "Pepe",
                        LastName = "Lopez",
                        Email = "blabl2a@gmail.com",
                        Password = "123456"
                    }

                };

                foreach (Client client in clients)
                {
                    context.Clients.Add(client);
                }

                
            }





            if (!context.Accounts.Any())
            {
                var clientNoe = context.Clients.FirstOrDefault(c => c.Email == "blabla@gmail.com");
               
                if (clientNoe != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = clientNoe.Id, CreationDate = DateTime.Now, Number = "00001", Balance = 100.000 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                   // context.SaveChanges();

                }


            }

            context.SaveChanges();
        }
    }
}
