using Aplication.Interfaces.Repository;
using Domain.Entities;
using Google.Cloud.Firestore;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly FirestoreDb _firestoreDb;
        protected readonly string _collectionName;
        private const int MaxRetries = 3;


        public GenericRepository(FirestoreDb firestoreDb, string collectionName)
        {
            _firestoreDb = firestoreDb;
            _collectionName = collectionName;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var document = _firestoreDb.Collection(_collectionName).Document(id);
             var snapchot = await document.GetSnapshotAsync();
            return snapchot.Exists ? snapchot.ConvertTo<T>() : null;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var querySnapshot = await _firestoreDb.Collection(_collectionName).GetSnapshotAsync();
            return querySnapshot.Documents.Select(doc => doc.ConvertTo<T>());

        }
        public virtual async Task<string?> AddAsync(T entity)
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                WriteResult result = await docRef.SetAsync(entity);
                var error = result.ToString();
                if (result is null)
                    return null;

                return docRef.Collection(_collectionName).Document().Id;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public virtual async Task<bool> UpdateAsync(string id, T entity)
        {
            await _firestoreDb.Collection(_collectionName).Document(id).SetAsync(entity);
            return true;
        }
        public virtual async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
                {
                    if (i == MaxRetries - 1)
                    {

                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            throw new Exception($"No se pudo completar la operación en {_collectionName} después de {MaxRetries} intentos");
        }
        public virtual async Task<bool> DeleteAsync(string id)
        {
            await _firestoreDb.Collection(_collectionName).Document(id).DeleteAsync();
            return true;
        }

        public Task<Profile?> GetProfileByIdAsync(string profileId)
        {
            throw new NotImplementedException();
        }
    }
}
