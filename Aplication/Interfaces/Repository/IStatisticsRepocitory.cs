using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IStatisticsRepocitory
    {
        Task<UsersByProvince?> GetUsersByProvince();
        Task<TotalStatics?> GetTotalStatics();
    }
}
