using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Aplication.Features.Profiles.Command.CreateProfile
{
    public class CreateProfileCommand : IRequest<Unit>
    {
        public string? Name { get; set; }
        public List<string> Users { get; set; } = new List<string>();
    }
}
