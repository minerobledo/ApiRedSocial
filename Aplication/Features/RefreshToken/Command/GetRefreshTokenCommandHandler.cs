using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using Aplication.DTO;
using Aplication.Interfaces.Services;
using Aplication.Interfaces.Repository;
using System.Runtime.CompilerServices;

namespace Aplication.Features.RefreshToken.Command
{
    public class GetRefreshTokenCommandHandler: IRequestHandler<GetRefreshTokenCommand,Response<RefreshTokenResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefresTokenRepository _refresTokenRepository;

        public GetRefreshTokenCommandHandler(IAuthService authService, IJwtTokenService jwtToken, IRefresTokenRepository refresTokenRepository) 
        {
            _authService = authService;
            _jwtTokenService = jwtToken;
            _refresTokenRepository = refresTokenRepository;
        }

        public async Task<Response<RefreshTokenResponseDto>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_authService.HasNullPropertiesLinq(request)) return new Response<RefreshTokenResponseDto>() { succeeded = false,message="credenciales invalidas" };
                var userId = _jwtTokenService.JWTReader(request.token);
                if (userId == null) return new Response<RefreshTokenResponseDto>() { succeeded = false, message = "credenciales invalidas" };

                var refreshToken = await _refresTokenRepository.ExistRefreshtoken(request.refreshToken);
                if (refreshToken == null) return new Response<RefreshTokenResponseDto>() { succeeded = false, message = "credenciales invalidas" };

                if(refreshToken.JwtToken == request.token && refreshToken.RefreshTokenValue == request.refreshToken)
                {
                    var newJWT = _jwtTokenService.GenerateToken(refreshToken.ProfileId!);
                    var newRefreshToken = _jwtTokenService.GenerateRefeshToken();
                   
                    refreshToken.RefreshTokenValue = newRefreshToken;
                    refreshToken.JwtToken = newJWT;
                    var update = await _refresTokenRepository.UpdateAsync(newRefreshToken, newJWT, refreshToken.Id!);
                    if (update == true)
                    {
                        return new Response<RefreshTokenResponseDto>() {
                            succeeded = true,
                            message = "tokens actualizados",
                            data = new RefreshTokenResponseDto() 
                            {
                                RefreshToken =newRefreshToken,
                                JWT = newJWT

                            } 
                        };
                    }
                }
                return new Response<RefreshTokenResponseDto>() 
                {
                    succeeded = false,
                    message = "cresenciales invalidas",
                    data = null,
                    
                    
                };


            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<RefreshTokenResponseDto>()
                {
                    succeeded = false,
                    message = "Error",
                    data = null,
                    errors = new List<Exception> { ex }
                };
            }
           
        }
    }
}
