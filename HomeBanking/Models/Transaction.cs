using System;

namespace HomeBanking.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public long AccountId { get; set; }
        //EFramework convención
        public Account Account { get; set; } 

    }
}
