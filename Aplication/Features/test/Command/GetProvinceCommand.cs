using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.test.Command
{
    public  class GetProvinceCommand : IRequest<List<Dictionary<string, object>>>
    {
        public string comand {  get; set; }
    }
}
