using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using MediatR;
using Aplication.DTO.Test;


using Aplication.Features.test.Command;
using Microsoft.AspNetCore.Cors;
namespace Api.Controllers
{
    [EnableCors("WebPolicy")]
    [ApiController]
    [Route("api/Test")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{mail}")]
        public async Task<IActionResult> Test(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return Unauthorized();

            }
            try
            {
                var comand = new SendEmailCommand(){Email=  mail};
                var response = await _mediator.Send(comand);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Registro invalido");
            }
        }
        [HttpPost("notificatios")]
        public async Task<IActionResult> Notification([FromBody] NotificationRequest notificationRequest) 
        {
            if (notificationRequest == null) { return Unauthorized(); }
            try
            {
                var comand = new SendNotificationCommand() { notificationRequest = notificationRequest };
                var response = await _mediator.Send(comand);
                return Ok(response);
            } catch (UnauthorizedAccessException)
            {
                return Unauthorized("algo malio sal");
            }

        }
        [HttpPost("GETprovince")]
        public async Task<IActionResult> GETprovince()
        {
            var comand = new GetProvinceCommand();
            var responce = await _mediator.Send(comand);
            return Ok(responce);
        }

    }
  

}
