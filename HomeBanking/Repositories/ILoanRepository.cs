using HomeBanking.Models;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ILoanRepository
    {
        Loan FindById(long id);
        IEnumerable<Loan> GetAllLoans();
    }
}
