using MediatR;
using Microsoft.AspNetCore.Mvc;
using Aplication.Features.Profiles.Command.Login;
using Aplication.Features.Profiles.Command.TokenLogin;
using Aplication.ResponPattern;
using Google.Apis.Auth.OAuth2.Requests;
using Aplication.Features.RefreshToken.Command;
using Microsoft.AspNetCore.Cors;
using Aplication.Features.Profiles.Command.Logout;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route(("api/auth"))]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            if (loginCommand == null)
            {
                return Unauthorized("credenciales  inválidas");
            }
            try
            {
                var response = await _mediator.Send(loginCommand);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("credenciales  inválidas");
            }
        }

        [HttpPost("TokenLogin")]

        public async Task<IActionResult> TokenLogin([FromBody] TokenLoginCommand tokenLoginCommand)
        {
            if (tokenLoginCommand == null)
            {
                return Unauthorized(new
                {
                    message = "Credenciales inválidas",
                    data = (object)null!
                });
            }

            try
            {
                var response = await _mediator.Send(tokenLoginCommand);

                // Respuesta exitosa con message y data
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                // Respuesta en caso de error con message y data vacío
                return Unauthorized(new
                {
                    message = "Credenciales inválidas",
                    data = (object)null!
                });
            }
            catch (Exception ex)
            {
                // Manejar otros errores inesperados
                return StatusCode(500, new
                {
                    message = "Ocurrió un error inesperado en TokenLogin",
                    data = ex.Message
                });
            }

        }

        [HttpPost("GetRefreshToken")]

        public async Task<IActionResult> RefreshToken([FromBody] GetRefreshTokenCommand request)
        {
            if (request == null)
            {
                return Unauthorized();
            }
            try
            {
                Console.WriteLine("el jwt es: " + request.token);
                Console.WriteLine("el refeshtoken es: " + request.refreshToken);
                var response = await _mediator.Send(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized("credenciales  inválidas");
            }
        }
        [HttpPost("Logout")]
   
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] LogoutCommand command)
        {
            try
            {
                command.Principal = User;
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("credenciales  inválidas");
            }
        }

        
    }
}
