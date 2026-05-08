using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using Domain.Entities.Chats;
using Google.Cloud.Firestore;


namespace Infrastructure.Repositories
{
    internal class StatisticsRepocitory: GenericRepository<object>, IStatisticsRepocitory
    {
        public StatisticsRepocitory(FirestoreDb firestoreDb, string collectionName = "Statistics") : base(firestoreDb, collectionName)
        {
            
        }

        public async Task<UsersByProvince?> GetUsersByProvince()
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document("UsersByProvince");
                var snapshot = await docRef.GetSnapshotAsync();
                return snapshot.ConvertTo<UsersByProvince>();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<TotalStatics?> GetTotalStatics()
        {
            try
            {
                var a = new TotalStatics();

                // Obtener las estadísticas originales
                var docRef = _firestoreDb.Collection(_collectionName).Document("UserByAge");
                var snapshot = await docRef.GetSnapshotAsync();
                var userByAgeRaw = snapshot.ToDictionary();
                a.UserByAge = userByAgeRaw;

                // Procesar por rangos de edad
                var ageRangeCounts = new Dictionary<string, int>()
        {
            { "18-20", 0 },
            { "21-25", 0 },
            { "26-30", 0 },
            { "31-35", 0 },
            { "36-40", 0 },
            { "41-45", 0 },
            { "46-50", 0 },
            { "51-55", 0 },
            { "56-60", 0 },
            { "61-65", 0 },
            { "66-70", 0 },
            { "71-75", 0 },
            { "76-80", 0 },
            { "80+", 0 }
        };

                foreach (var entry in userByAgeRaw)
                {
                    if (int.TryParse(entry.Key, out int age) && entry.Value is long count)
                    {
                        string range = age switch
                        {
                            >= 18 and <= 20 => "18-20",
                            >= 21 and <= 25 => "21-25",
                            >= 26 and <= 30 => "26-30",
                            >= 31 and <= 35 => "31-35",
                            >= 36 and <= 40 => "36-40",
                            >= 41 and <= 45 => "41-45",
                            >= 46 and <= 50 => "46-50",
                            >= 51 and <= 55 => "51-55",
                            >= 56 and <= 60 => "56-60",
                            >= 61 and <= 65 => "61-65",
                            >= 66 and <= 70 => "66-70",
                            >= 71 and <= 75 => "71-75",
                            >= 76 and <= 80 => "76-80",
                            > 80 => "80+",
                            _ => null
                        };

                        if (range != null)
                        {
                            ageRangeCounts[range] += (int)count;
                        }
                    }
                }

                // Puedes devolver este diccionario o incluirlo dentro de `TotalStatics`
                a.UserByAgeRange = ageRangeCounts;

                // Resto de las estadísticas
                var docRef1 = _firestoreDb.Collection(_collectionName).Document("UserOrientation");
                var snapshot1 = await docRef1.GetSnapshotAsync();
                a.UserByOrientation = snapshot1.ToDictionary();

                var docRef2 = _firestoreDb.Collection(_collectionName).Document("UserBySex");
                var snapshot2 = await docRef2.GetSnapshotAsync();
                a.UserBySex = snapshot2.ToDictionary();

                var docRef3 = _firestoreDb.Collection(_collectionName).Document("UsersByProvince");
                var snapshot3 = await docRef3.GetSnapshotAsync();
                a.UsersByProvince = snapshot3.ToDictionary();

                return a;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
