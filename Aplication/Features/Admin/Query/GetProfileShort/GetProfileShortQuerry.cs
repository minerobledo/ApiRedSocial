using Aplication.DTO.OutputDto.Profile;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Query.GetProfileShort
{
    public class GetProfileShortQuerry : IRequest<Response<List<ProfileShortDto>>>
    {
        public Dictionary<string , object>? Parameters { get; set; }

        public DateTime? StardtAfter {  get; set; }

    }
}
