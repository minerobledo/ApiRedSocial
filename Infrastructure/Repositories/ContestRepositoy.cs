using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Infrastructure.Services.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Infrastructure.Repositories
{
    internal class ContestRepositoy : GenericRepository<Contest>, IContestRespository
    {
        private readonly IQuartzJobService _quartzJobService;
        public ContestRepositoy(IQuartzJobService quartzJobService, FirestoreDb firestoreDb, string collectionName = "Contests") : base(firestoreDb, collectionName)
        {
            _quartzJobService = quartzJobService;
        }

        public async Task<bool?> AddPostIdToContest(string contestId, string postId)
        {
            if (string.IsNullOrWhiteSpace(contestId) || string.IsNullOrWhiteSpace(postId)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(contestId);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    var contest = snapshot.ConvertTo<Contest>();
                    if (contest.PostId == null)
                    {
                        contest.PostId = new List<string>();
                    }
                    contest.PostId.Add(postId);
                    docRef.SetAsync(contest);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<Contest?> GetContestById(string id)
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<Contest>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        public async Task<bool?> FinishedContest(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var dic = new Dictionary<string, object>()
                {
                    {"State","close"}
                };
                await docRef.UpdateAsync(dic);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool?> CreateContest(Contest contest)
        {

            if (contest == null) return null;

            try
            {
                var docRef = await _firestoreDb.Collection(_collectionName).AddAsync(contest);
                var dic = new Dictionary<string, object>()
                {
                    {"ContestID",docRef.Id }
                };
                if(contest.StartDate.Value.Date == DateTime.UtcNow.Date)
                {
                    _quartzJobService.AddTask<ConquestFinaliceJob>(docRef.Id+ "-contest-job", contest.EndDate.Value,dic);
                }
                else
                {
                    _quartzJobService.AddTask<StartContestJob>(docRef.Id + "-contest-job", contest.StartDate.Value, dic);
                }

                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool?> DeleteContest(string contestId)
        {
            if (string.IsNullOrWhiteSpace(contestId)) return null;
            try
            {
                var doc = await _firestoreDb.Collection(_collectionName).Document(contestId).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool?> EditContes(Contest contest)
        {
            if (contest == null) return null;
            try
            {
                var doc = await _firestoreDb.Collection(_collectionName).Document(contest.Id).SetAsync(contest);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }

        }
        public async Task<List<Contest>?> GetContestFinishedToday(DateTime dateTime)
        {

            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("EndDate", dateTime);
                var snapshot =await query.GetSnapshotAsync();
                var list = new List<Contest>();
                foreach (var contest in snapshot)
                {
                    list.Add(contest.ConvertTo<Contest>());
                }
                return list;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<bool?> AddDaysToContest(string contestId , int days)
        {
            if (!string.IsNullOrWhiteSpace(contestId) || days == 0) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(contestId);
                var snapshot = await docRef.GetSnapshotAsync();
                var a  = snapshot.ConvertTo<Contest>();
                a.EndDate = a.EndDate.Value.AddDays(days);
                await docRef.SetAsync(a);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<Contest>?> GetWorkinContestsPaginated(DateTime? dateTime)
        {
            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    .WhereEqualTo("State", "working") // Filtrar por estado "Workin"
                    .OrderByDescending("CreateAt") // Ordenar por fecha de creación más reciente primero
                    .Limit(10); // Limitar resultados para paginación

                // Si se envía un `lastDocumentDate`, continuar la paginación desde ahí
                if (dateTime.HasValue)
                {
                    query = query.StartAfter(dateTime.Value);
                }

                var snapshot = await query.GetSnapshotAsync();
                var contests = new List<Contest>();

                foreach (var doc in snapshot.Documents)
                {
                    contests.Add(doc.ConvertTo<Contest>());
                }

                return contests;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<Contest>?> GetContestsAdminPaginated(DateTime? dateTime)
        {
            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    
                    .OrderByDescending("CreateAt") // Ordenar por fecha de creación más reciente primero
                    .Limit(10); // Limitar resultados para paginación

                // Si se envía un `lastDocumentDate`, continuar la paginación desde ahí
                if (dateTime.HasValue)
                {
                    query = query.StartAfter(dateTime.Value);
                }

                var snapshot = await query.GetSnapshotAsync();
                var contests = new List<Contest>();

                foreach (var doc in snapshot.Documents)
                {
                    contests.Add(doc.ConvertTo<Contest>());
                }

                return contests;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> StartContest(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var dic = new Dictionary<string, object>()
                {
                    {"State","working"}
                };
                await docRef.UpdateAsync(dic);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<Contest>> GetContestToInit()
        {
            var now = Timestamp.FromDateTime(DateTime.UtcNow);

            var snapshot = await _firestoreDb.Collection("Contests")
                .WhereEqualTo("State", "pending")
                //.WhereGreaterThan("startDate", now)
                .GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Contest>()).ToList();
        }

        public async Task<List<Contest>> GetContestsToFinalaiz()
        {
            var now = Timestamp.FromDateTime(DateTime.UtcNow);

            var snapshot = await _firestoreDb.Collection("Contests")
                .WhereEqualTo("State", "working")
                //.WhereGreaterThan("endDate", now)
                .GetSnapshotAsync();

            return snapshot.Documents.Select(d => d.ConvertTo<Contest>()).ToList();
        }


    }

}
