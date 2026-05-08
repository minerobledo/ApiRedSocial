using Aplication.DTO.OutputDto.Admin;
using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Login
{
    internal class AdminLoginCommandHaandler:IRequestHandler<AdminLoginCommand,Response<LoginAdminDto>>
    {
        private readonly IAdminRepocitory _adminRepocitory;
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefresTokenRepository _refresTokenRepository;

        public AdminLoginCommandHaandler(IAdminRepocitory adminRepocitory, IAuthService authService, IJwtTokenService jwtTokenService, IRefresTokenRepository refresTokenRepository)
        {
            _adminRepocitory = adminRepocitory;
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _refresTokenRepository = refresTokenRepository;
        }

        public async Task<Response<LoginAdminDto>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var self = await _adminRepocitory.GetAdminByTokenLogin(request.TokenLogin!);
                if (self != null)
                {
                    bool flagEmail = self.Email == request.Email ? true : false;
                    bool flagPasswword = _authService.AuthenticatePasswordEncript(self.Password!,request.Password!);
                    if (flagEmail && flagPasswword)
                    {
                        var refreshToken = _jwtTokenService.GenerateRefeshToken();
                        var jwt = _jwtTokenService.GenerateAdminToken(self.Id!);
                        string? refresID = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(self.Email!,null);
                        if (refresID == null)
                        {
                            var add = await _refresTokenRepository.AddDocumentAsync(self.Email!, self.Id!, refreshToken, jwt, null);
                            if (add == null || add == false)
                            {
                                return new Response<LoginAdminDto> { succeeded = false, message = "Error en bace de datos" };
                            };
                        }
                        else if (!string.IsNullOrWhiteSpace(refresID))
                        {
                            var flag = await _refresTokenRepository.UpdateAsync(refreshToken, jwt, refresID);
                            if (flag == null || flag == false)
                            {
                                return new Response<LoginAdminDto> { succeeded = false, message = "Error en bace de datos" };
                            }

                        }
                        return new Response<LoginAdminDto>
                        {
                            succeeded = true,
                            data = new LoginAdminDto
                            {
                                RefreshToken = refreshToken,
                                JWT = jwt,
                                Id = self.Id,
                                Name = self.Name,
                                LastName = self.LastName
                            }
                        };
                    }
                    
                }
                return new Response<LoginAdminDto> { succeeded = false, message = "Credenciales invalidas 3" };
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
                return new Response<LoginAdminDto> { succeeded = false, message = "Error", errors = new List<Exception> { ex } };
            }
        }


    }
}
