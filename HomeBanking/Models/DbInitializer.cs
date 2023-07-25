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

            context.SaveChanges();
        }
    }
}
