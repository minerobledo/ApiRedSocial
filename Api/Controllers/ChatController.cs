using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aplication.Features.Chat.Command.CreateChat;
using Aplication.Interfaces.Repository;
using Aplication.Features.Chat.Querry.GetMessageFromChat;
using Aplication.Features.Chat.Querry.GetChatsByProfile;
using Aplication.DTO.InputDto.Chat;
using Microsoft.AspNetCore.Cors;
using Aplication.Features.Chat.Querry.GetAllAdminChats;
using Aplication.Features.Chat.Command.CreateAdminChat;

namespace Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        protected readonly IMediator _mediatr;
        public ChatController(IMediator mediator)
        {
            _mediatr = mediator;
        }

        [HttpPost("CreateChat")]
    
        [Authorize]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto dto)
        {
            if (dto == null) { return Unauthorized("credenciales invalidas"); }
            try
            {
                var command = new CreateChatCommand() {
                    Principal =User,
                    SelfProfileName = dto.SelfProfileName,
                    FriendProfileName = dto.FriendProfileName,
                    Profile2Id = dto.FriendId
                };
                var responce = await _mediatr.Send(command);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateAdminChat")]
        
        [Authorize]
        public async Task<IActionResult> CreateChat([FromBody] CreateAdminChatCommand command)
        {
            try
            {
                command.Principal = User;
                var responce = await _mediatr.Send(command);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("Messages")]
        
        [Authorize]
        public async Task<IActionResult> GetMessages([FromBody] GetMessageFromChatQuerry querry)
        {
            try
            {
                var responce = await _mediatr.Send(querry);
                return Ok(responce);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetChatByProfile")]
       
        [Authorize]
        public async Task<IActionResult> GetChatByProfile()
        {
            try
            {
                var query = new GetChatsByProfileQuerry { principal = User };
                var responce = await _mediatr.Send(query);
                return Ok(responce);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("GetAllAdminChats/{date}")]
        
        [Authorize]
        public async Task<IActionResult> GetAllAdminChats([FromQuery] DateTime? date = null)
        {
            try
            {
                var query = new GetAllAdminChatsQuery { Principal = User , StartAfter = date};
                var responce = await _mediatr.Send(query);
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
