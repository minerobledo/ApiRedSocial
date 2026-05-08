using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Command.AcceptDeny
{
    internal class AcceptDenyCommandHandler: IRequestHandler<AcceptDenyCommand, Response<bool?>>
    {
        private readonly IPostRepository _repository;

        public AcceptDenyCommandHandler(IPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<bool?>> Handle(AcceptDenyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.AcceptDeny == true)
                {
                    var result = await _repository.UpdatePostById(request.Post.Id, request.Post);
                    return new Response<bool?> { succeeded = true, data = result };
                }
                else
                {
                    var result = await _repository.DeletePostById(request.Post.Id);
                    return new Response<bool?> { succeeded = true, data = result };
                }
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
;
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
