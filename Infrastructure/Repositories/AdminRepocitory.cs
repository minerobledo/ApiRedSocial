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
    internal class AdminRepocitory : GenericRepository<Admin>, IAdminRepocitory
    {
        public AdminRepocitory(FirestoreDb firestoreDb, string collectionName = "Admin") : base(firestoreDb, collectionName) { }

        public async Task<Admin?> GetAdminById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    return snapshot.ConvertTo<Admin>();
                }
                return null;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> CreateAdmin(string email, string password,string name, string Lname)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                var admin = new Admin()
                {
                    Email = email,
                    Password = password,
                    TokenLogin = await GenerateUniqueTokenLoginAsync(),
                    Name = name,
                    LastName = Lname
                    

                };
                var result = await docRef.SetAsync(admin);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> ExistAdminByID( string id)
        {
            if(string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection( _collectionName).Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists) return true;

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool?> ExistAdminByTokenLogin(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenLogin",token);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count==1) return true;

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Admin?> GetAdminByTokenLogin(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenLogin", token);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count == 1) return snapshot[0].ConvertTo<Admin>();

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        // funciones internas 
        private async Task<string> GenerateUniqueTokenLoginAsync()
        {
            string code;
            bool isUnique;

            do
            {
                code = GenerateSixDigitCode();
                isUnique = await IsUniqueTokenLoginAsync(code);
            }
            while (!isUnique);

            return code;
        }
        private static string GenerateSixDigitCode()
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000);
            return code.ToString("D6");
        }
        
        private async Task<bool> IsUniqueTokenLoginAsync(string code)
        {
            var CodeRepetTokenLogin = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenLogin", code);

            var listCodesTokenLogin = await CodeRepetTokenLogin.GetSnapshotAsync();
            if (!listCodesTokenLogin.Any())
                return true;

            return false;

        }
    }
}
