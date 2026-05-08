using Aplication.Interfaces.Services;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class TransactionService : ITransactionService
    {
        private readonly FirestoreDb _db;
        public TransactionService(FirestoreDb db)
        {
            _db = db;
        }
        public async Task<bool> ExecuteTransactionAsync(Func<Transaction, Task> operation)
        {
            try
            {
                await _db.RunTransactionAsync(async transaction =>
                {
                    await operation(transaction);
                });
                return true;
            }
            catch  
            {
                return false;
            }
        }
    }
}
