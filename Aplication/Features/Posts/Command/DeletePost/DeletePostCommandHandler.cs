using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Command.DeletePost
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Response<bool?>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IJwtTokenService _jwtTokenService;
        public DeletePostCommandHandler(IJwtTokenService jwtTokenService, IPostRepository postRepository)
        {
         
            _jwtTokenService = jwtTokenService;
            _postRepository = postRepository;

        }

        public async Task<Response<bool?>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var publisherId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var flag = await _postRepository.DeletePostWithIpPublisher(request.PostId, publisherId);

                return new Response<bool?>()
                {
                    succeeded = true,
                    data = flag
                };
            }catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }

                return new Response<bool?>()
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };
            }

        }
    }
}
