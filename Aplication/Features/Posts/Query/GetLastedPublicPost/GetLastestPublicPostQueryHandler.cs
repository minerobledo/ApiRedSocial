using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetLastedPublicPost
{
    internal class GetLastestPublicPostQueryHandler : IRequestHandler<GetLastestPublicPostQuery, Response<List<Post>?>>
    {
        private readonly IPostRepository _postRepository;
        public GetLastestPublicPostQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;

        }

        public async Task<Response<List<Post>?>> Handle(GetLastestPublicPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _postRepository.GetLastPublicPostPaginated(request.date);
                return new Response<List<Post>?>()
                {
                    succeeded = true,
                    data = list
                };

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

                return new Response<List<Post>?>()
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };
            }
        }
    }
}
