using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetLastedFriendsPost
{
    public class GetLastestFriendsPostQueryHandler : IRequestHandler<GetLastestFriendsPostQuery, Response<List<Post>?>>
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public GetLastestFriendsPostQueryHandler(IJwtTokenService jwtTokenService, IPostRepository postRepository,IFriendshipRepository friendshipRepository)
        {
            _jwtTokenService = jwtTokenService;
            _friendshipRepository = friendshipRepository;
            _postRepository = postRepository;
        }

        public async Task<Response<List<Post>?>> Handle(GetLastestFriendsPostQuery request, CancellationToken cancellationToken)
        {
            var mainId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            var listIds = await _friendshipRepository.GetAllFriendsIDpByProfilIdAsinc(mainId);
            try
            {
                var list = await _postRepository.GetLastedFriendsPostPaginated(request.date,listIds);
                if(list!= null)
                {
                    return new Response<List<Post>?> { data = list, succeeded = true };
                }
                return new Response<List<Post>?> { data = new List<Post>() , succeeded = true };
            }
            catch (Exception ex)
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

                return new Response<List<Post>?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
