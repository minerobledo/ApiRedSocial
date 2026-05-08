using Aplication.DTO.InputDto.Chat;
using Aplication.DTO.InputDto.Friendship;
using Aplication.DTO.InputDto.Profile;
using Aplication.Features.Chat.Querry.GetMessageFromChat;
using Aplication.Features.File;
using Aplication.Features.Frinship.Command.ResonceFriendshipRequest;
using Aplication.Features.Frinship.Command.SendFriendshipRequest;
using Aplication.Features.Frinship.Querys;
using Aplication.Features.Notificatoins.Command;
using Aplication.Features.Profiles.Command.AcceptDenyGodathering;
using Aplication.Features.Profiles.Command.ChangeInterest;
using Aplication.Features.Profiles.Command.DeleteProfile;
using Aplication.Features.Profiles.Command.DeleteTrustedDevice;
using Aplication.Features.Profiles.Command.Register;
using Aplication.Features.Profiles.Command.SetTrustedDevice;
using Aplication.Features.Profiles.Command.UpdateGeoPoint;
using Aplication.Features.Profiles.Command.UpdateProfil;
using Aplication.Features.Profiles.Command.VerifyProfile;
using Aplication.Features.Profiles.Queries.EsistTokenGodfather;
using Aplication.Features.Profiles.Queries.ExistProfilByEmail;
using Aplication.Features.Profiles.Queries.ExistProfilByPhoneNumber;
using Aplication.Features.Profiles.Queries.ExistProfileName;
using Aplication.Features.Profiles.Queries.GetProfiles;
using Aplication.Features.Profiles.Queries.GetProfilesThatVerify;
using Aplication.Features.Profiles.Queries.GetTrustedDevice;
using Aplication.Features.Profiles.Queries.SerchProfile;
using Aplication.Features.Reports.Command.ReportProfile;
using Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Index.HPRtree;
using System.Security.Claims;


namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost("ExistProfileName")]
        
        public async Task<IActionResult> ExistProfileName([FromBody] ExistProfileNameQuery existProfileNameQuery)
        {
            if (existProfileNameQuery == null)
            {
                return Unauthorized("Accion invalida");
            }
            try
            {
                var responce = await _mediator.Send(existProfileNameQuery);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("ExistTokenGodfather")]
        
        public async Task<IActionResult> ExistTokengodfather([FromBody] ExistTokenGodfatherQuery existTokenGodfatherQuery)
        {
            if (existTokenGodfatherQuery == null)
            {
                return Unauthorized("Accion invalida");
            }
            try
            {
                var responce = await _mediator.Send(existTokenGodfatherQuery);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("ExistEmail/{email}")]
        
        public async Task<IActionResult> ExistProfileEmail([FromRoute] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
            try
            {
                ExistProfileByEmailQuery exist = new ExistProfileByEmailQuery() { EmailToCheck = email };
                var responce = await _mediator.Send(exist);
                return Ok(responce);

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ExistPhonenumber/{phone}")]
        
        public async Task<IActionResult> ExistProfilePhonNumber([FromRoute] string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return Unauthorized();
            try
            {
                var command = new ExistProfileByPhoneNumberQuerry { PhoneToCheck = phone };
                var responce = await _mediator.Send(command);
                return Ok(responce);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("RegisterProfile")]
       
        public async Task<IActionResult> RegisterProfil([FromBody] RegisterCommand registerCommand)
        {
            if (registerCommand == null) return Unauthorized("credenciales Inbalidas");
            try
            {
                var responce = await _mediator.Send(registerCommand);
                return Ok(responce);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("ChangeInterest/{interes}")]
        
        [Authorize]
        public async Task<IActionResult> ChangeInterest([FromRoute] string interes)
        {
            if (string.IsNullOrWhiteSpace(interes)) { return Unauthorized("credenciales invalidas"); }
            try
            {
                var command = new ChangeInterestCommand { principal = User, Interest = interes };
                var responce = await _mediator.Send(command);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


        }

       

        [HttpGet("SendFriendRequest/{profileIdReseptor}")]
        
        [Authorize]
        public async Task<IActionResult> SendFriendRequest([FromRoute] string profileIdReseptor)
        {


            if (profileIdReseptor == null) { return Unauthorized("credenciales invalidas"); }

            try
            {
                var comman = new SendFriendshipRequestCommand()
                {
                    Principal = User,
                    ProfileIdReseptor = profileIdReseptor

                };
                var responce = await _mediator.Send(comman);
                return Ok(responce);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ResponceFriendRequest")]
        
        [Authorize]
        public async Task<IActionResult> ResponceFriendRequest([FromBody] ResponceFriendshipRequestDTO dto)
        {
            if (dto.FriendshipId == null || dto.Responce == null) { return Unauthorized("credenbciales invalidas"); }
            try
            {
                var command = new ResonceFriendshipRequestCommand()
                {
                    principal = User,
                    FriendshipId = dto.FriendshipId,
                    Responce = dto.Responce
                };
                var responce = await _mediator.Send(command);
                return Ok(responce);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetFriend/{profileId}")]
        [Authorize]
        public async Task<IActionResult> GetFrienchips([FromRoute] string profileId)
        {

            if (profileId == null) return Unauthorized("Credenciales invalidas");
            try
            {
                var querry = new GetFrienchipsQuery() { ProfileId = profileId };
                var responce = await _mediator.Send(querry);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProfileLong/{profileName}")]
        [Authorize]
        public async Task<IActionResult> GetProfile([FromRoute] string profileName)
        {
            if (string.IsNullOrEmpty(profileName)) return Unauthorized("Credenciales invalidas");
            try
            {
                var query = new GetProfilesLongQuery() { profileName = profileName, ClaimsPrincipal = User };

                var profiles = await _mediator.Send(query);

                return Ok(profiles);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("SearchProfile/{profilName}")]
        [Authorize]
        public async Task<IActionResult> SearchProfile([FromRoute] string profilName)
        {
            if (string.IsNullOrEmpty(profilName)) return Unauthorized("Credenciales invalidas");
            try
            {
                var query = new SerchProfileQuery() { Principal = User, Name = profilName };

                var profiles = await _mediator.Send(query);

                return Ok(profiles);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileEditDto dto)
        {
            if (dto == null) return Unauthorized("Credenciales invalidas");
            try
            {

                // Validar tamaño (ej: max 5MB)
                if (dto.CoverPhoto != null && dto.CoverPhoto.Length > 20 * 1024 * 1024  )
                    return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");


                if (dto.CoverPhoto != null)
                {
                    // Validar extensión
                    var ext = Path.GetExtension(dto.CoverPhoto.FileName).ToLower();
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    if (!allowedExts.Contains(ext))
                        return BadRequest("Formato de archivo no permitido");

                    // Validar MIME type
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(dto.CoverPhoto.ContentType))
                        return BadRequest("Tipo de archivo no permitido");

                }
                if (dto.ProfilePhoto != null)
                {

                    if (dto.ProfilePhoto.Length > 20 * 1024 * 1024)
                        return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");

                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    // Validar extensión
                    var ext1 = Path.GetExtension(dto.ProfilePhoto.FileName).ToLower();
                    var allowedExts1 = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    if (!allowedExts1.Contains(ext1))
                        return BadRequest("Formato de archivo no permitido");

                    // Validar MIME type
                    var allowedTypes1 = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes1.Contains(dto.ProfilePhoto.ContentType))
                        return BadRequest("Tipo de archivo no permitido");
                }


                var propiedades = dto.GetType().GetProperties();

                foreach (var prop in propiedades)
                {
                    var valor = prop.GetValue(dto, null);
                    Console.WriteLine($"{prop.Name}: {valor}");
                }
                var command = new UpdateProfileCommand()
                {
                    Principal = User,
                    profileEdit = dto
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("UpdateGeoPoin")]
     
        [Authorize]
        public async Task<IActionResult> UpdateGeoPoin([FromBody] UpdateGeoPoinCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteProfile")]
       
        [Authorize]
        public async Task<IActionResult> DeleteProfile([FromBody] DeleteProfileCommand command) 
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("ReportProfile")]

        [Authorize]
        public async Task<IActionResult> ReportProfile([FromBody] ReportProfileCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("SetTrustedDevice")]

        [Authorize]
        public async Task<IActionResult> SetTrustedDevice([FromBody] SetTrustedDeviceCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("GetTrustedDevice")]
        
        [Authorize]
        public async Task<IActionResult> GetTrustedDevice([FromBody] GetTrustedDeviceQuery command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("DeleteTrustedDevice")]
        
        [Authorize]
        public async Task<IActionResult> DeleteTrustedDevice([FromBody] DeleteTrustedDeviceCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.Principal = User;
                var result = await _mediator.Send(command);
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
                var result = await _mediator.Send(command);
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
                
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("AcceptDeniGodfathering")]

        [Authorize]
        public async Task<IActionResult> GetProfilesThatVerif([FromBody] AcceptDenyGodatheringCommand command)
        {
            if (command == null) return Unauthorized("Credenciales invalidas");
            try
            {
                command.principal = User;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("UploadComprobante")]
        [Authorize]
        public async Task<IActionResult> UploadComprobante([FromForm] ComprobanteDTO dto)
        {
            if (dto.FormFile == null || dto.FormFile.Length == 0)
                return BadRequest("No se envió archivo");

            // Validar tamaño (ej: max 10MB para PDF)
            if (dto.FormFile.Length > 10 * 1024 * 1024)
                return BadRequest("El archivo excede el tamaño máximo permitido (10MB)");

            // Validar extensión
            var ext = Path.GetExtension(dto.FormFile.FileName).ToLower();
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
            if (!allowedExts.Contains(ext))
                return BadRequest("Formato de archivo no permitido");

            // Validar MIME type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "application/pdf" };
            if (!allowedTypes.Contains(dto.FormFile.ContentType))
                return BadRequest("Tipo de archivo no permitido");

            // Validar magic number para PDF
            if (ext == ".pdf")
            {
                using (var reader = new BinaryReader(dto.FormFile.OpenReadStream()))
                {
                    byte[] buffer = reader.ReadBytes(5); // leer primeros 5 bytes
                    string header = System.Text.Encoding.ASCII.GetString(buffer);

                    if (!header.StartsWith("%PDF-"))
                        return BadRequest("El archivo no es un PDF válido");
                }
            }
            try
            {
                var command = new UploadCommand()
                {
                    FileName = dto.FormFile.FileName,
                    ContentType = dto.FormFile.ContentType,
                    Stream = dto.FormFile.OpenReadStream()
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
