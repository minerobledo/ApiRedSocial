
using Aplication.DTO.InputDto.Register;
using Aplication.Features.Profiles.Command.Register;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    
    [ApiController]
    [Route("api/Register")]
    public class RegisterController :ControllerBase
    {
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        
        public async Task<IActionResult> Register([FromForm] RegisterCommand recuestRegisterDto)
        {
            if (recuestRegisterDto == null)
            {
                return Unauthorized("Registro invalido");
            }
        
            if (recuestRegisterDto.FacePhotoUser1 == null || recuestRegisterDto.FacePhotoUser1 == null || recuestRegisterDto.FacePhotoUser1.Length == 0)
                return BadRequest("No se envió archivo");

            // Validar tamaño (ej: max 5MB)
            if (recuestRegisterDto.FacePhotoUser1.Length > 20 * 1024 * 1024)
                return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");

            // Validar extensión
           

            // Validar MIME type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(recuestRegisterDto.FacePhotoUser1.ContentType))
                return BadRequest("Tipo de archivo no permitido");

            if (recuestRegisterDto.cantidadUsuraios == 2)
            {
                if (recuestRegisterDto.FacePhotoUser2 == null || recuestRegisterDto.FacePhotoUser2 == null || recuestRegisterDto.FacePhotoUser2.Length == 0)
                    return BadRequest("No se envió archivo");

                // Validar tamaño (ej: max 5MB)
                if (recuestRegisterDto.FacePhotoUser2.Length > 20 * 1024 * 1024)
                    return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");

                // Validar extensión
               

                // Validar MIME type
           
                if (!allowedTypes.Contains(recuestRegisterDto.FacePhotoUser2.ContentType))
                    return BadRequest("Tipo de archivo no permitido");
            }
          
            try
            {
                var response = await _mediator.Send(recuestRegisterDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Registro invalido");
            }

        }
       
    }
}
