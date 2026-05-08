using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Post
{
    public class GetLastFriendPostDato
    {
        public DateTime? Date {  get; set; }

        public List<string>? Ids { get; set; }
    }
}
