using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Register
{
    internal class RegisterAdminCommandHandler: IRequestHandler<RegisterAdminCommand, Response<bool?>>
    {
        private readonly IAdminRepocitory _adminRepocitory;
        private readonly IAuthService _authService;

        public RegisterAdminCommandHandler(IAdminRepocitory adminRepocitory, IAuthService authService)
        {
            _adminRepocitory = adminRepocitory;
            _authService = authService;
        }

        public async Task<Response<bool?>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            var flagPassword = _authService.ValidatePassword(request.Password!);
            try
            {
                if (flagPassword)
                {
                    var pas = _authService.HashinPassword(request.Password!);
                    var a = await _adminRepocitory.CreateAdmin(request.Email!, pas, request.Name!, request.LastnameName!);
                    return new Response<bool?> { succeeded = true, data = a };

                }
                return new Response<bool?> { succeeded = true, data = false };
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
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };

            }
        }
    }
}
