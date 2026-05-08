using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Command.UploadPost
{
    public class UploadPostCommandHandler : IRequestHandler<UploadPostCommand, Response<Post?>>
    {
        private readonly IFileService _fileService;
        private readonly IContestRespository _contestRespository;
        private readonly IPostRepository _postRepository;
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public UploadPostCommandHandler(IFileService fileService, IContestRespository contestRespository, IPostRepository postRepository, IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _fileService = fileService;
            _contestRespository = contestRespository;
            _postRepository = postRepository;
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<Post?>> Handle(UploadPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var profileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);

             
               

                // generar nombre único en el bucket
                string fileName = $"posts/{Guid.NewGuid()}_{Path.GetFileName(request.FileName)}";

                // subir archivo al storage
                var url = await _fileService.UploadFileAsync(request.PhotoStream, fileName, request.ContentType);

                // crear post en DB
                var post = new Post
                {
                    IdPublisher = profileId,
                    ProfileNamePublisher = request.ProfileName,
                    Url = url,
                    Status = "pending",
                    PostType = request.PostType,
                    CreateAt = DateTime.UtcNow,
                    Description = request.Description,
                    ContestId = request.ContestId
                };

                post.Id = await _postRepository.UploadPost(post);

                // si pertenece a un concurso, agregar referencia
                if (request.ContestId != null)
                {
                    await _contestRespository.AddPostIdToContest(request.ContestId, post.Id);
                }

                return new Response<Post?> { succeeded = true, data = post };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }

                return new Response<Post?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
