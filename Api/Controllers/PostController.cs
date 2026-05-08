using Aplication.DTO.InputDto.Post;
using Aplication.Features.Posts.Command.DeletePost;
using Aplication.Features.Posts.Command.LikeDislike;
using Aplication.Features.Posts.Command.UploadPost;
using Aplication.Features.Posts.Query.GetLastedFriendsPost;
using Aplication.Features.Posts.Query.GetLastedPublicPost;
using Aplication.Features.Posts.Query.GetPostByPorfile;
using Aplication.Features.Posts.Query.GetPostList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    [ApiController]
    [Route(("api/[controller]"))]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("UploadPost")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UploadPost([FromForm] PostToUpload post)
        {

            if (post == null || post.File == null || post.File.Length == 0)
                return BadRequest("No se envió archivo");

            // Validar tamaño (ej: max 5MB)
            if (post.File.Length > 20 * 1024 * 1024)
                return BadRequest("El archivo excede el tamaño máximo permitido (5MB)");

            // Validar extensión
            var ext = Path.GetExtension(post.File.FileName).ToLower();
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowedExts.Contains(ext))
                return BadRequest("Formato de archivo no permitido");

            // Validar MIME type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(post.File.ContentType))
                return BadRequest("Tipo de archivo no permitido");

            try
            {
                // Acá consumís el stream del archivo
                using var stream = post.File.OpenReadStream();

                var command = new UploadPostCommand
                {
                    Principal = User,
                    ProfileName = post.ProfileName,
                    PostType = post.PostType,
                    Description = post.Description,
                    PhotoStream = stream,  // le pasás el stream en vez del IFormFile
                    FileName = post.File.FileName,
                    ContentType = post.File.ContentType,
                    ContestId = post.ContestId
                };

                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete ("DeletePost/{postId}")]
     
        [Authorize]
        public async Task<IActionResult> DeletePost([FromRoute] string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                return Unauthorized("credenciales  inválidas");
            }
            try
            {
                var command = new DeletePostCommand()
                {
                    Principal = User,
                    PostId = postId
                };
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("LikeDislikePost")]
        
        [Authorize]
        public async Task<IActionResult> LikeDislikePost([FromBody] LikeDislikeDto dto)
        {
            if(dto == null)
            {
                return Unauthorized("credenciales  inválidas");
            }
            try
            {
                var command = new LikeDislikeCoommand()
                {
                    Principal = User,
                    PostId = dto.PostID,
                    State = dto.State
                };
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


        }

        [HttpGet("GetLastestPublicPosts/{dateTime}")]
     
        [Authorize]
        public async Task<IActionResult> GetLastestPublicPosts([FromQuery] DateTime? dateTime = null)
        {
            var query = new GetLastestPublicPostQuery();
            if (dateTime.HasValue)
            {
                query.date = dateTime;
            }
            try
            {
                var responce = await _mediator.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetLastestFriendsPosts/{dateTime}")]
        
        [Authorize]
        public async Task<IActionResult> GetLastedFriendPosts([FromQuery] DateTime? dateTime = null)
        {
            
            var query = new GetLastestFriendsPostQuery();
            if (dateTime.HasValue)
            {
                query.date = dateTime;
            }
            query.Principal= User;
            try
            {
                var responce = await _mediator.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("GetPostList")]
       
        [Authorize]
        public async Task<IActionResult> GetPostList([FromBody]List<string> ids)
        {
            if (ids== null) return Unauthorized("credenciales  inválidas");
            var query = new GetPostListQuery() { PostList = ids};
          
            try
            {
                var responce = await _mediator.Send(query);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetPostByProfile/{id}")]
   
        [Authorize]
        public async Task<IActionResult> GetPostByProfile([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return Unauthorized("credenciales  inválidas");
            var query = new GetPostByProfileQuery() { ProfileId = id,Principal = User };

            try
            {
                var responce = await _mediator.Send(query);
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
