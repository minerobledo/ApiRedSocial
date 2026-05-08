using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Aplication.DTO.InputDto.Contest;
using Aplication.Features.Contests.Command.CreateContest;
using Domain.Entities;
using Aplication.Features.Contests.Command.EdutContest;
using Aplication.Features.Contests.Command.DeleteContest;
using Aplication.Features.Contests.Querys.GetContest;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ContestController : ControllerBase
    {
        protected readonly IMediator _mediatr;
        public ContestController(IMediator mediator) 
        {
            _mediatr = mediator;
        }

        
   

        
        [HttpGet("GetContest/{dateTime}")]
      
        [Authorize]
        public async Task<IActionResult> GetContest([FromQuery] DateTime dateTime )
        {
            try
            {
                var query  = new GetContestQuery() { DateTime = dateTime };
                var result = await _mediatr.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
