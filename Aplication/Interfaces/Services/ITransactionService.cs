using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Aplication.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<bool> ExecuteTransactionAsync(Func<Transaction, Task> operation);
    }
}
