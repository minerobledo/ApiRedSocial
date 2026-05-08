using AutoMapper;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aplication.Features.Notificatoins.Command.ResponceRequestFacePhoto;
using Aplication.Features.Notificatoins.Command.RequestFacePhoto;
using Aplication.Features.Notificatoins.Query;
using MediatR;
using Aplication.Features.Notificatoins.Command;
using Aplication.DTO.InputDto.Notificaution;
using Microsoft.AspNetCore.Cors;
using Aplication.Features.Notificatoins.Query.GetGlovalNotifications;
using Aplication.Features.Notificatoins.Command.ReadNotification;
using Aplication.Features.Notificatoins.Command.DeleteNotification;
namespace Api.Controllers
{

    [ApiController]
    [Route(("api/Notifications"))]
    public class NotificationControler: ControllerBase
    {
        protected readonly IMediator _mediatr;
        public NotificationControler(IMediator mediatr) 
        {
            _mediatr = mediatr;
        }
        [HttpGet("GetGlovalNotifications")]
        
        [Authorize]
        public async Task<IActionResult> GetGlovalNotifications()
        {
            

            try
            {
                var query = new GetGlovalNotificationsQuerry();
                var response = await _mediatr.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ReadNotifications")]
 
        [Authorize]
        public async Task<IActionResult> ReadNotifications([FromBody]ReadNotificationCommand command)
        {
            try
            {
                var response = await _mediatr.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetNotificationsByProfilID")]
      
        [Authorize]
        public async Task<IActionResult> GetNotificationsByProfilID()
        {
            var query = new GetNotificationByProfilIdQuery();
            query.Principal = User;

            
            try
            {
                var response = await _mediatr.Send(query);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteNotification")]

        [Authorize]
        public async Task<IActionResult> ResponceRequestFacePhoto([FromBody] DeleteNotificationCommand command)
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
        [HttpPost("SendRequestFacePhoto")]
       
        [Authorize]
        public async Task<IActionResult> SendRequestFacePhoto([FromBody] SendRequestFacePhotoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.profilId) || string.IsNullOrWhiteSpace(dto.selfName)) return Unauthorized("credenciales  inválidas");
            try
            {
                var command = new RequestFacePhotoCommand() { ProfilId = dto.profilId, Principal = User ,SelfName = dto.selfName};
                var responce = await _mediatr.Send(command);
                return Ok(responce);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("ResponseRequestFacePhoto")]
     
        [Authorize]
        public async Task<IActionResult> ResponceRequestFacePhoto([FromBody] ResponceRequestFacePhotoDto dto)
        {
            if (dto == null) return Unauthorized("credenciales  inválidas");
            try
            {
                var command = new ResponseRequestFacePhotoCommand()
                {
                    Principal = User,
                    idSender = dto.idSender,
                    NotificationId = dto.NotificationId,
                    response = dto.responce
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
    }
}
