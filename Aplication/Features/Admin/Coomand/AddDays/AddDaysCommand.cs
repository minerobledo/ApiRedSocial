using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.AddDays
{
    public class AddDaysCommand: IRequest<Response<bool?>>
    {
        public int Days {  get; set; }
        public string? ProfileID { get; set; }

    }
}
