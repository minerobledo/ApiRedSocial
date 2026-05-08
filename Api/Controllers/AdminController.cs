using Aplication.DTO.OutputDto.Admin;
using Aplication.Features.Admin.Coomand.Login;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Aplication.Features.Admin.Coomand.Register;
using Microsoft.AspNetCore.Authorization;
using Aplication.Features.Admin.Coomand.DeleteEverithing;
using Aplication.Features.Admin.Coomand.AddDays;
using Aplication.Features.Admin.Query.GetPendingPost;
using Aplication.Features.Posts.Command.AcceptDeny;

using Aplication.Features.Event.Command.CreateEvent;
using Aplication.Features.Event.Command.UpdateEvent;
using Aplication.Features.Admin.Coomand.Penalize;
using Aplication.Features.Admin.Query.GetProfileShort;
using Aplication.Features.Admin.Query.GetProfileById;
using Aplication.Features.Chat.Querry.GetChatsBystateForAdmin;
using Aplication.Features.Chat.Command.ActiveAdminChat;
using Aplication.Features.Chat.Command.CloseAdminChat;
using Aplication.Features.Event.query.GetEventsPendingPaginated;
using Aplication.Features.Event.query.GetEventsAceptedPaginated;
using Aplication.Features.Map.Querys.GetMap;
using Aplication.Features.Event.Command.AcceptDeny;
using Aplication.Features.Admin.Query.GetTotalStatics;
using Aplication.Features.Contests.Command.DeleteContest;
using Aplication.Features.Contests.Command.EdutContest;
using Domain.Entities;
using Aplication.Features.Contests.Querys.GetContestByState;
using Aplication.Features.Posts.Query.GetPostList;
using Aplication.Features.Profiles.Command.VerifyProfile;
using Aplication.Features.Profiles.Queries.GetProfilesThatVerify;
using Aplication.DTO.InputDto.Contest;
using Aplication.Features.Contests.Command.CreateContest;
using Aplication.Features.Jobs.Querys.GetJobsProgramed;
using Aplication.Features.Reports.Query.GetReportByFilter;
using Aplication.Features.Reports.Command.ChangeStateReport;
using Aplication.Features.Chat.Querry.GetMessageFromChat;
using Aplication.Features.Profiles.Command.Logout;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController: ControllerBase
    {
        protected readonly IMediator _mediatr;
        public AdminController(IMediator mediator)
        {
            _mediatr = mediator;
        }

      

        [HttpPost("Login")]
        
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AdminLoginCommand dto)
        {
            if (dto == null) return Unauthorized("credenciales invalidas");
            try
            {
                var result = await _mediatr.Send(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("Logout")]
        
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutAdminCommand dto)
        {
            if (dto == null) return Unauthorized("credenciales invalidas");
            try
            {
                dto.Principal = User;
                var result = await _mediatr.Send(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("RegisterAdmin")]
        
        [Authorize]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminCommand dto)
        {
            if (dto == null) return Unauthorized("credenciales invalidas");
            try
            {
                var result = await _mediatr.Send(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("Penalize")]
        
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] PenalizeProfileCommand command)
        {
            if (command == null) return Unauthorized("credenciales invalidas");
            try
            {  
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("GetProfileShort")]
       
        [Authorize]
        public async Task<IActionResult> GetProfileShort([FromBody] GetProfileShortQuerry command)
        {
            if (command == null) return Unauthorized("credenciales invalidas");
            try
            {
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("GetProfileLong")]
      
        [Authorize]
        public async Task<IActionResult> GetProfileLong([FromBody] GetProfileByIdQuery command)
        {
            if (command == null) return Unauthorized("credenciales invalidas");
            try
            {
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("Delete")]

        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteEverithingCommand command)
        {
            if (command == null) return Unauthorized("credenciales invalidas");
            try
            {
                command.principal = User;
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("AddDays")]

        [Authorize]
        public async Task<IActionResult> AddDays([FromBody] AddDaysCommand command)
        {
            if(command == null) return Unauthorized("credenciales invalidas");
            try
            {
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("VerifyProfile")]
      
        [Authorize]
        public async Task<IActionResult> VerifyProfile([FromBody] VerifyProfileCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("GetProfilesThatVerif")]
  
        [Authorize]
        public async Task<IActionResult> GetProfilesThatVerif([FromBody] GetProfilesThatVerifyQuery command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {

                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //post
        [HttpGet("GetPostPending/{date?}")][Authorize]
        public async Task<IActionResult> GetPostPending([FromRoute] DateTime date)
        {
            
            try
            {
                var command = new GetPendingPostCommand()
                {
                    dateTime = date
                };
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
      
        [HttpPost("AcceptDenyPost")][Authorize]
        public async Task<IActionResult> AcceptDenyPost([FromBody] AcceptDenyCommand command)
        {
            try
            { 
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("GetPostList")]
 
        [Authorize]
        public async Task<IActionResult> GetPostList([FromBody] List<string> ids)
        {
            if (ids == null) return Unauthorized("credenciales  inválidas");
            var query = new GetPostListQuery() { PostList = ids };

            try
            {
                var responce = await _mediatr.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        
        //event
        [HttpGet("GetEventsAceptedPaginated/{date?}")][Authorize]
        public async Task<IActionResult> GetEventsAceptedPaginated([FromRoute] DateTime? date = null)
        {
            try
            {
                var query = new GetEventsAceptedPaginatedQuery() { Date = date };
                var responce = await _mediatr.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("GetEventsPendingPaginated/{date?}")][Authorize]
        public async Task<IActionResult> GetEventsPendingPaginated([FromRoute] DateTime? date = null)
        {
            try
            {
                var query = new GetEventsPendingPaginatedQuery() { Date = date };
                var responce = await _mediatr.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        [HttpPost("AcceptDenyEvents")][Authorize]
        public async Task<IActionResult> AcceptDenyEvents([FromBody] AcceptDenyEventCommand command)
        {
            try
            {
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPost("CreateEvent")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventCommand dto)
        {
            if (dto.BanerFile == null || dto.BanerFile == null || dto.BanerFile.Length == 0)
                return BadRequest("No se envió archivo");

            // Validar tamaño (ej: max 5MB)
            if (dto.BanerFile.Length > 20 * 1024 * 1024)
                return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");

            // Validar extensión
            var ext = Path.GetExtension(dto.BanerFile.FileName).ToLower();
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowedExts.Contains(ext))
                return BadRequest("Formato de archivo no permitido");

            // Validar MIME type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(dto.BanerFile.ContentType))
                return BadRequest("Tipo de archivo no permitido");
            try
            {
                var responce = await _mediatr.Send(dto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("UpdateEvent")][Authorize]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventCommand dto)
        {
            if (dto == null)
            {
                return Unauthorized("credenciales invalidas");
            }
            try
            {
                var responce = await _mediatr.Send(dto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        //chat

        [HttpPost("GetChatsByState")][Authorize]
        public async Task<IActionResult> GetChatsByState([FromBody] GetChatsByStateQuerry dto)
        {
            if (dto == null)
            {
                return Unauthorized("credenciales invalidas");
            }
            try
            {
                var responce = await _mediatr.Send(dto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("ActiveAdminChat")]

        [Authorize]
        public async Task<IActionResult> GetChatsByState([FromBody] ActiveAdminChatCommand dto)
        {
            if (dto == null)
            {
                return Unauthorized("credenciales invalidas");
            }
            try
            {
                var responce = await _mediatr.Send(dto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPost("CloseAdminChat")]

        [Authorize]
        public async Task<IActionResult> GetChatsByState([FromBody] CloseAdminChatCommand dto)
        {
            if (dto == null)
            {
                return Unauthorized("credenciales invalidas");
            }
            try
            {
                var responce = await _mediatr.Send(dto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //Contest
        [HttpDelete("DeleteContest/{id}")]
 
        [Authorize]
        public async Task<IActionResult> DeleteContest([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return Unauthorized("credenciales invalidas"); }
            try
            {
                var command = new DeleteContestCommand() { id = id };
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("EditContest")]

        [Authorize]
        public async Task<IActionResult> EditContest([FromBody] Contest contest)
        {
            if (contest == null) { return Unauthorized("credenciales invalidas"); }
            try
            {
                var command = new EditContestCommand() { Contest = contest };
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("GetContestAdmin")]
   
        [Authorize]
        public async Task<IActionResult> EditContest([FromBody] GetContestAdminQuery command)
        {
            try
            {
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("CreateContest")]

        [AllowAnonymous]
        public async Task<IActionResult> CreateContest([FromBody] ContestToCreate contest)
        {
            if (contest == null) { return Unauthorized("credenciales invalidas"); }
            try
            {
                var command = new CreateContestCommand() { contestToCreate = contest };
                var result = await _mediatr.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        //statics
        [HttpGet("GetMap")]
        
        [Authorize]
        public async Task<IActionResult> GetMap()
        {
            
            try
            {
                var responce = await _mediatr.Send(new GetMapQuery());
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("GetTotalStatics")]
       
        [Authorize]
        public async Task<IActionResult> GetTotalStatics()
        {
            try
            {
                var responce = await _mediatr.Send(new GetTotalStaticsQuery());
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //jobs
        [HttpGet("GetJobsProgreamed")]
       
        [Authorize]
        public async Task<IActionResult> GetJobsProgreamed()
        {
            try
            {
                var query = new GetJobsProgramedQuery();
                var responce = await _mediatr.Send(query);
                return Ok(responce);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        //repotrts
        [HttpPost("GetReportByFilter")]
     
        [Authorize]
        public async Task<IActionResult> GetReportByFilter([FromBody] GetReportByFilterQuerry querry)
        {
            try
            {
                var responce = await _mediatr.Send(querry);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPost("ChangeStateReport")]

        [Authorize]
        public async Task<IActionResult> GetReportByFilter([FromBody] ChangeStateReportCommand querry)
        {
            try
            {
                querry.Principal = User;
                var responce = await _mediatr.Send(querry);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        //chats
        [HttpPost("Messages")]

        [Authorize]
        public async Task<IActionResult> GetMessages([FromBody] GetMessageFromChatQuerry querry)
        {
            try
            {
                var responce = await _mediatr.Send(querry);
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
