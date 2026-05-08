using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Penalize
{
    public class PenalizeProfileCommand: IRequest<Response<bool?>>
    {
        public string? Function {  get; set; }
        public string? ProfileId { get; set; }
        public string? BanReason { get; set; }
        public int DeysToRemuve { get; set; }
        public DateTime UnBanDate { get; set; }
    }
}
