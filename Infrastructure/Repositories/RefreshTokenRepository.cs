using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Google.Cloud.Firestore;
using Aplication.Interfaces.Repository;

namespace Infrastructure.Repositories
{
    internal class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefresTokenRepository
    {
       
        public RefreshTokenRepository(FirestoreDb firestoreDb, string collectionName = "RefreshToken") : base(firestoreDb, collectionName)
        {
            
        }

        public async Task<bool?> AddDocumentAsync(string userEmail, string profileId, string refreshToken,string jwt,int? user = null)
        {
            
            try
            {
                
                if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(profileId)) return false;
                
                var docRef = await _firestoreDb.Collection(_collectionName).AddAsync(new RefreshToken()
                {
                    UserEmail = userEmail,
                    ProfileId = profileId,
                    RefreshTokenValue = refreshToken,
                    JwtToken = jwt,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    User = user
                    

                });
                
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> UpdateAsync(string RefreshTokenValue, string JwtToken, string documentID)
        {
            if (string.IsNullOrWhiteSpace(RefreshTokenValue) ||
                string.IsNullOrWhiteSpace(RefreshTokenValue) ||
                string.IsNullOrWhiteSpace(documentID))
            {
                return false;
            }
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(documentID);                
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    {"JwtToken",JwtToken },
                    {"ExpiresAt",DateTime.UtcNow.AddDays(7) },
                    {"RefreshTokenValue",RefreshTokenValue }
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task DeleteRefeshtoken(string id)
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<RefreshToken?> ExistRefreshtoken(string refresh)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refresh)) return null;
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("RefreshTokenValue", refresh);
                var docRef = await query.GetSnapshotAsync();
                if (docRef.Any()&& docRef.Count == 1)
                {
                    return docRef.First().ConvertTo<RefreshToken>();
                }
                return null;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<string?> GetRefresTokenDocumentIdIfExist(string id,int? user = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return null;

                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("ProfileId", id);
                query = query.WhereEqualTo("User", user);
                var docRef = await query.GetSnapshotAsync();
                if (docRef.Any() && docRef.Count == 1)
                {
                    return docRef.First().Id;
                }
                if(docRef.Count > 1)
                {
                    foreach (var doc in docRef)
                    {
                        await doc.Reference.DeleteAsync();
                    }
                }
                return null;

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
           
        }

        public Task<bool?> AddDocumentAsync(string userId, string profileId, string refreshToken, string jwt)
        {
            throw new NotImplementedException();
        }
    }
}
