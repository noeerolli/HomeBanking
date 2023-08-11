using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ICardRepository
    {
        void Save(Card card);

        Card FindById(long Id);
        IEnumerable<Card> GetAllCards();
        IEnumerable<Card> GetCardsByClients(long clientId);

        
        
    }
}
