using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;

namespace Aplication.Features.Posts.Query.GetPostByPorfile
{
    public class GetPostByProfileQueryHandler : IRequestHandler<GetPostByProfileQuery, Response<List<Post>?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        
        

        public GetPostByProfileQueryHandler(IJwtTokenService jwtTokenService,IPostRepository postRepository, IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
            _jwtTokenService = jwtTokenService;
            _postRepository = postRepository;
        }

        public async Task<Response<List<Post>?>> Handle(GetPostByProfileQuery request, CancellationToken cancellationToken)
        {
            var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal!);
            try
            {

                if (selfId == request.ProfileId)
                {
                    var a =await _postRepository.GetSelfProfilePosts(request.ProfileId!);
                    return new Response<List<Post>?> { succeeded = true, data = a};
                }
                var flag = await _friendshipRepository.ExistFrienship(selfId!, request.ProfileId!);
                if (flag ==true)
                {
                    var a = await _postRepository.GetFriendProfilePosts(request.ProfileId!);
                    return new Response<List<Post>?> { succeeded = true, data = a };
                }
                var b = await _postRepository.GetAceptedPublicProfilePosts(request.ProfileId!);
                if(b != null)
                {
                    return new Response<List<Post>?> { succeeded = true, data = b};
                }
                return new Response<List<Post>?> { succeeded = true, data = new List<Post>() };
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Ocurrió una excepción:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return new Response<List<Post>?> { succeeded = false, errors = new List<Exception>() { ex } };

            }
        }
    }
}
