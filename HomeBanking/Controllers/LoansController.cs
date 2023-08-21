using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {

        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        public LoansController(ILoanRepository loanRepository, IClientRepository clientRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository) 
        {
            _loanRepository = loanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }


        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid("Email vacio");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }


                //verificamos que el prestamo exista	
                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);

                if (loan == null)
                {
                    return BadRequest("Prestamo no valido");
                }

                //verifica q el monto NO sea 0 y que no sobrepase el máximo autorizado.
                if (loanApplicationDTO.Amount == 0 || loanApplicationDTO.Amount > loan.MaxAmount)
                {
                    return BadRequest("Error en el monto");            
                }


                //verifica q los payments no lleguen vacíos / o sean incorrectos

                
                var paymentList = loan.Payments.Split(',');

                if (!paymentList.Contains(loanApplicationDTO.Payments))
                {
                    return BadRequest("Error en las cuotas solicitadas");
                }

                 //verifica q  exista la cuenta de destino

                 var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                
                 if (account == null)
                 {
                    return Forbid();
                 }

                 //verifica q la cuenta de destino pertenezca al Cliente autenticado.

                 var clientAcc = client.Accounts.Where(acc => acc.Number == account.Number)
                   .FirstOrDefault();

                 if (clientAcc == null)
                 {
                    
                    return BadRequest("La cuenta no pertenece al cliente");
                 }


                //Actualizar el Balance de la cuenta sumando el monto del préstamo.
                account.Balance += loanApplicationDTO.Amount;
                _accountRepository.Save(account);


                Transaction transaction = new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanApplicationDTO.Amount,
                    Description = loan.Name.ToString() + " Prestamo Aprobado",
                    Date = DateTime.Now,
                    AccountId = account.Id
                };

                _transactionRepository.Save(transaction);

                ClientLoan clientLoan = new ClientLoan
                {
                    Amount = loanApplicationDTO.Amount + loanApplicationDTO.Amount * 0.2,
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loan.Id
                };


                _clientLoanRepository.Save(clientLoan);

                return Ok(loanApplicationDTO);


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }



        }



        [HttpGet]

        public IActionResult Get()   //IActionResult->tipo de dato que devuelve una respuesta para el navegador

        {

            try

            {

                var loans = _loanRepository.GetAllLoans();  



                var loansDTO = new List<LoanDTO>();



                foreach (Loan loan in loans)

                {

                    LoanDTO loanDTO = new LoanDTO
                    {

                        Id = loan.Id,

                        MaxAmount = loan.MaxAmount,

                        Name = loan.Name,

                        Payments = loan.Payments,

                    };  


                    loansDTO.Add(loanDTO);

                }


                return Ok(loansDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }





    }
}
