using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetPostList
{
    public class GetPostListQueryHandler : IRequestHandler<GetPostListQuery, Response<List<Domain.Entities.Post>?>>
    {
        private readonly IPostRepository _postRepository;
        public GetPostListQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Response<List<Post>?>> Handle(GetPostListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _postRepository.GetPostByList(request.PostList);
                if (a == null)
                {
                    return new Response<List<Post>?>()
                    {
                        succeeded = true,
                        message = "error en base se datos"
                    };

                }
                else
                {
                    return new Response<List<Post>?>()
                    {
                        succeeded = true,
                        data = a
                    };
                }
            } catch (Exception ex)
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

                return new Response<List<Post>?>()
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };
            }
        }
    }
}
