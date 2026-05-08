using Aplication.DTO.InputDto.Event;
using Aplication.Features.Event.Command.AddOrRemubeGuest;
using Aplication.Features.Event.Command.AcceptDeny;
using Aplication.Features.Event.Command.CreateEvent;
using Aplication.Features.Event.Command.UpdateEvent;
using Aplication.Features.Event.query.GetEventsAceptedPaginated;
using Aplication.Features.Event.query.GetEventsPendingPaginated;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route(("api/Event"))]
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        

        

        

        [HttpGet("GetEventsAceptedPaginated/{date}")]
  
        [Authorize]
        public async Task<IActionResult> GetEventsAceptedPaginated([FromQuery] DateTime? date)
        {
            try
            {
                var query = new GetEventsAceptedPaginatedQuery() {Date = date };
                var responce = await _mediator.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

       

        [HttpPost("AddRemuveGuestToEvent")]
    
        [Authorize]
        public async Task<IActionResult> AddRemuveGuestToEvent([FromBody] AddOrRemubeGuestCommand command)
        {
            
            try
            {
               command.Principal = User;
                var responce = await _mediator.Send(command);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
