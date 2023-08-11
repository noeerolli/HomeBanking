﻿using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        

        public AccountsController(IAccountRepository accountRepository)

        {

            _accountRepository = accountRepository;
            
        }



        [HttpGet]

        public IActionResult Get()
        

        {

            try

            {

                var accounts = _accountRepository.GetAllAccounts();



                var accountsDTO = new List<AccountDTO>();



                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO

                    {

                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(trans => new TransactionDTO

                        {

                            Id = trans.Id,

                            Type = trans.Type,

                            Amount = trans.Amount,

                            Description = trans.Description,

                            Date = trans.Date

                        }).ToList()

                    };



                    accountsDTO.Add(newAccountDTO);

                }



                return Ok(accountsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }



        [HttpGet("{id}")]

        public IActionResult Get(long id)

        {

            try

            {

                var account = _accountRepository.FindById(id);

                if (account == null)

                {

                    return NotFound();

                }



                var accountDTO = new AccountDTO

                {

                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(trans => new TransactionDTO

                    {

                        Id = trans.Id,

                        Type = trans.Type,

                        Amount = trans.Amount,

                        Description = trans.Description,

                        Date = trans.Date

                    }).ToList()

                };



                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }


        [HttpPost]
        public IActionResult Post(long clientId)
        {
            try
            {
                Account newAccount = new Account
                {
                    ClientId = clientId,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    Number = "VIN-" + new Random().Next(100000, 999999).ToString(),

                };
                _accountRepository.Save(newAccount);
                AccountDTO newAccDTO = new AccountDTO
                {
                    Id = newAccount.Id,
                    Balance = newAccount.Balance,
                    CreationDate = newAccount.CreationDate,
                    Number = newAccount.Number
                };
                return Created("", newAccDTO);
                
            }
            catch
            {
                return null;
            }

        }



    }
}

