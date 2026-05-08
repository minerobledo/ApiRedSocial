using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IReportRepository
    {
        Task<bool?> AddReport(Report report);
        Task<bool?> DeleteReport(string id);
        Task<bool?> ChangeState(string id, string state, string? adminId = null, string? result = null);
        Task<List<Report>?> GetReportByFilterAsync(Dictionary<string, object> filter, DateTime? startAfterId = null);
    }

}
