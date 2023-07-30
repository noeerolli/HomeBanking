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
                var clientNoe = context.Clients.FirstOrDefault(c => c.Email == "blabla@gmail.com");   //el param de la función es lo que devuelve el método FirstOrDefault(en este caso un Client)
               
                if (clientNoe != null)
                {
                    var accounts = new Account[]
                    {
                        new Account 
                        {
                            ClientId = clientNoe.Id, CreationDate = DateTime.Now, Number = "00001", Balance = 100.000 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                   // context.SaveChanges();

                }


            }


            if (!context.Transactions.Any())

            {

                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "00001");

                if (account1 != null)

                {

                    var transactions = new Transaction[]

                    {

                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda Despegar", Type = TransactionType.DEBIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda Nubes", Type = TransactionType.DEBIT.ToString() },

                    };

                    foreach (Transaction transaction in transactions)

                    {

                        context.Transactions.Add(transaction);

                    }

                    context.SaveChanges();



                }

            }




            context.SaveChanges();
        }
    }
}
