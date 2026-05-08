using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Aplication.Features.Profiles.Queries.ExistProfileName;
using Aplication.Features.Map.Querys.GetMap;
using Aplication.Features.Map.Command.Get;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapController: ControllerBase
    {
        private readonly IMediator _mediator;

        public MapController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetMap")]
    
        [Authorize]
        public async Task<IActionResult> GetMap()
        {
            try
            {
                var query = new GetMapQuery();
                var responce = await _mediator.Send(query);
                return Ok(responce);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }
       

        [HttpPost("GetNearbyProfil")]
     
        [Authorize]
        public async Task<IActionResult> GetNearbyProfil([FromBody] GetNearbyProfilesCommand command)
        {
            if(command == null) return Unauthorized("credenciales  inválidas");
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
