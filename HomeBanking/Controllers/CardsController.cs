using HomeBanking.Models;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {

        private ICardRepository _cardRepository;
        private ClientsController _clientsController;

        public CardsController(ICardRepository cardRepository)
        {

            _cardRepository = cardRepository;
            
        }



        public IActionResult Post(Card newCard)//ver q pasar
        {
            try
            {


                _cardRepository.Save(newCard);

                CardDTO newCardDTO = new CardDTO
                {
                    Id = newCard.Id,
                    CardHolder = newCard.CardHolder,
                    Color = newCard.Color,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    Number = newCard.Number,
                    ThruDate = newCard.ThruDate,
                    Type = newCard.Type
                };
                return Created("", newCardDTO);


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }










        }

    }
}