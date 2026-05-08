using Aplication.DTO.OutputDto.Profile;
using Domain.Entities;
using MediatR;

namespace Aplication.Features.Admin.Query.GetProfileById
{
    public class GetProfileByIdQuery : IRequest<Response<ProfileForAdmin?>>
    {
        public string? ProfileId { get; set; }


    }
}
