using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IContestRespository
    {
        Task<bool?> CreateContest(Contest contest);
        Task<bool?> DeleteContest(string contestId);
        Task<bool?> AddPostIdToContest(string contestId , string postId);
        Task<bool?> EditContes(Contest contest);
        Task<List<Contest>?> GetContestFinishedToday(DateTime dateTime);
        Task<bool?> AddDaysToContest(string contestId, int days);
        Task<List<Contest>?> GetWorkinContestsPaginated(DateTime? dateTime);
        Task<List<Contest>?> GetContestsAdminPaginated( DateTime? dateTime);
        Task<Contest?> GetContestById(string id);
        Task<bool?> FinishedContest(string id);
        Task<bool?> StartContest(string id);
        Task<List<Contest>> GetContestToInit();
        Task<List<Contest>> GetContestsToFinalaiz();
    }
}
