using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HomeBanking.Controllers



{
    [Route("api/[controller]")]    

    [ApiController]

    public class ClientsController : ControllerBase

    {

        private IClientRepository _clientRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        


        public ClientsController(IClientRepository clientRepository,  AccountsController accountsController, CardsController cardsController) //el clientController necesita para su creacion de un repositorio

        {
            _clientRepository = clientRepository;       //el clientRepository q estamos recibiendo por el constructor, lo asignamos a la propiedad privada, para luego poder usarlo en el controlador
            _accountsController = accountsController;
            _cardsController = cardsController;
        }
        //ya tenemos acceso a los métodos definidos en la interfaz



        [HttpGet]

        public IActionResult Get()   //IActionResult->tipo de dato que devuelve una respuesta para el navegador

        {

            try

            {

                var clients = _clientRepository.GetAllClients();  //método de nuestro repositorio de clientes



                var clientsDTO = new List<ClientDTO>();  



                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO   //por cada client creamos un nuevo ClientDto
                    {

                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO     //por cada cuenta se crea un nuevo accountDTO

                        {

                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),          //guardamos todo en una lista dentro de Accounts
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type
                        }).ToList()

                    };



                    clientsDTO.Add(newClientDTO);

                }





                return Ok(clientsDTO);

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

                var client = _clientRepository.FindById(id);

                if (client == null)

                {

                    return NotFound();

                }



                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()

                };


                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }



        [HttpGet("current")]      //cuando el cliente ya esta logueado se hace la petición a get current - si esta todo correcto traemos todos sus datos
        public IActionResult GetCurrent()
        {
            try
            {       //chequeamos si la persona es cliente con la info que trae la cookie 
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;     //chequeamos q el mail no esté vacío/ devuelve el valor del reclamo (el cual configuramos sea el mail)
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);     //chequeamos si se encuentra en nuestra base de datos

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO         //si encontramos al cliente traemos todos los datos
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]   
        public IActionResult Post([FromBody] Client client)    //creamos un cliente, recibimos los datos en el cuerpo de la solicitud
        {
            try
            {

                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(400, "datos inválidos");



                //verificar que nombre y apellido tengan al menos 3 caracteres
                if (client.FirstName.Length < 3 || client.LastName.Length < 3)
                {
                    
                    return StatusCode(400, "Nombre y apellido deben tener mas de 2 letras");
                }
                //verificar q no tenga carcteres especiales nombre y apell
                if (!Regex.IsMatch(client.FirstName, @"^[a-zA-Z\s]+$") || !Regex.IsMatch(client.LastName, @"^[a-zA-Z\s]+$"))
                {
                    
                    return StatusCode(400, "Error en los datos");
                }

                //verificar si el email es válido
                if (!(Regex.IsMatch(client.Email, @"^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$")))
                {
                    
                    return StatusCode(400, "Email invalido");
                }


                //pass debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número
                if (!(client.Password.Length >= 8 && client.Password.Any(char.IsUpper) && client.Password.Any(char.IsDigit) && client.Password.Any(char.IsLower)))
                {
                    
                    return StatusCode(403, "La contraseña debe tener al menos 8 digitos, una letra mayuscula, una minuscula y un numero");
                }




                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);
                _accountsController.Post(newClient.Id);    // enviamos para crear cuenta al nuevo cliente


                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("current/accounts")]      //trae todas las cuentas del cliente
        public IActionResult GetAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();
                }

                var accounts = client.Accounts;
                return Ok(accounts);            //devuelve una respuesta 200 y los datos
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("current/accounts")]     //p/ crear cuenta - c/ post al accountsController
        public IActionResult PostAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();
                }

                if (client.Accounts.Count > 2)
                {
                    return StatusCode(403, "Ya posse el tope maximo de 3 cuentas");
                }

                var account = _accountsController.Post(client.Id);

                if (account == null)
                {
                    return StatusCode(500, "Error al crear la cuenta");
                }
                return Created("", account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        

        [HttpPost("current/cards")]   //p/ crear tarjeta - c/ post al cardsController
        public IActionResult PostCards([FromBody] Card card)
        {
            try
            {
                
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);
                
                if (client == null)
                {
                    return NotFound();
                }
                if(card.Type != CardType.CREDIT.ToString() && card.Type != CardType.DEBIT.ToString())
                {
                    return BadRequest("El tipo de tarjeta no es valido");
                }
                if (card.Color != CardColor.GOLD.ToString() && card.Color != CardColor.SILVER.ToString() && card.Type != CardColor.SILVER.ToString())
                {
                    return BadRequest("El color de tarjeta no es valido");
                }
                
                
                int CardCount = client.Cards.Where(c => c.Type == card.Type).Count();
                if (CardCount > 2)
                {
                    return StatusCode(403, "Ya tiene 3 tarjetas del mismo tipo");
                }
                int sameCard = client.Cards.Where(c => card.Color == card.Color && c.Type ==card.Type).Count();
                if (sameCard == 1)
                {
                    return StatusCode(403, "Ya tiene una tarjeta del mismo tipo y color");
                }



                Card newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Color = card.Color,
                    Cvv = new Random().Next(100000, 999999),
                    FromDate = DateTime.Now,
                    Number = new Random().Next(100000, 999999).ToString() + "-" +
                             new Random().Next(100000, 999999).ToString() + "-" +
                             new Random().Next(100000, 999999).ToString() + " " +
                             new Random().Next(100000, 999999).ToString(),
                    ThruDate = DateTime.Now.AddYears(5),
                    Type = card.Type

                };

                var newCardDTO = _cardsController.Post(newCard);

                if(newCard == null)
                {
                    return StatusCode(500, "Error en la información");
                }
                return Created("", newCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("current/cards")] // traer todas las tarjetas
        public IActionResult GetCards()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();
                }

                var cards = client.Cards;
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




    }
}













