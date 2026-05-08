using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTO;
using Aplication.ResponPattern;
using MediatR;

namespace Aplication.Features.RefreshToken.Command
{
    public class GetRefreshTokenCommand : IRequest<Response< RefreshTokenResponseDto>>
    {
        public string refreshToken {  get; set; }
        public string token { get; set; }
    }
}
