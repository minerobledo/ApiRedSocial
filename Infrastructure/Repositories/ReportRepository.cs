using Aplication.Interfaces.Repository;
using Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(FirestoreDb firestoreDb, string collectionName = "Reports") : base(firestoreDb, collectionName)
        {

        }

        public async Task<bool?> AddReport(Report report)
        {
            if (report == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                await docRef.SetAsync(report);
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> DeleteReport(string id)
        {
            if (id == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> ChangeState(string id, string state,string? adminId = null, string? result = null)
        {
            if (string.IsNullOrEmpty(state)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                if(state == "closed")
                {
                    await docRef.UpdateAsync(new Dictionary<string, object> {
                        { "State", state },
                        { "ClosedAt",DateTime.UtcNow},
                        { "Result", result}
                    });
                }
                else
                {
                    await docRef.UpdateAsync(new Dictionary<string,object> {
                        { "State", state },
                        { "AdminId", adminId }
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<Report>?> GetReportByFilterAsync(Dictionary<string, object> filter, DateTime? startAfterId = null)
        {

            try
            {
                Query query = _firestoreDb.Collection(_collectionName);

                // Agregar los filtros dinámicamente
                foreach (var filtro in filter)
                {
                    switch (filtro.Key)
                    {
                        case "State":
                            // code block
                            query = query.WhereEqualTo(filtro.Key, filtro.Value.ToString());
                            break;
                        case "Type":
                            // code block
                            query = query.WhereEqualTo(filtro.Key, filtro.Value.ToString());
                            break;

                        case "ReporterProfileName":
                            query = query.WhereEqualTo(filtro.Key, filtro.Value.ToString());
                            break;
                        case "ReportedProfileName":
                            query = query.WhereEqualTo(filtro.Key, filtro.Value.ToString());
                            break;
                       
                        default:
                            // code block
                            query = query.WhereEqualTo(filtro.Key, filtro.Value);
                            break;
                    }
                }

                // Ordenar por un campo conocido (por ejemplo "CreateAt" o "Id")
                query = query.OrderByDescending("CreateAt");

                // Agregar paginación si viene un ID del último documento
                if (startAfterId.HasValue)
                {
                    query = query.StartAfter(startAfterId);
                }


                // Limitar los resultados
                query = query.Limit(10);

                // Ejecutar la consulta
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                List<Report> result = new List<Report>();
                foreach (var doc in snapshot.Documents)
                {
                    result.Add(doc.ConvertTo<Report>());
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
    }
}
