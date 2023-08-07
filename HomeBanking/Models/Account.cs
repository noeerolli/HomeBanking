using System;
using System.Collections.Generic;

namespace HomeBanking.Models
{
    public class Account
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }

        public Client Client { get; set; }  //referencia a la entidad de la cual depende

        public long ClientId { get; set; }  // referencia a la clase de la cual depende

        public ICollection<Transaction> Transactions { get; set; }
    }
}
