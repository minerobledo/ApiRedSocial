using Aplication.Features.Jobs.Querys.GetJobsProgramed;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route(("api/[controller]"))]
    public class JobController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
    }
}
