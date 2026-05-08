using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TotalStatics
    {
        public Dictionary<string, object>? UserByAge { get; set; }
        public Dictionary<string, int>? UserByAgeRange { get; set; }
        public Dictionary<string, object>? UserByOrientation { get; set; }
        public Dictionary<string, object>? UserBySex { get; set; }
        public Dictionary<string, object>? UsersByProvince { get; set; }
    }
}
