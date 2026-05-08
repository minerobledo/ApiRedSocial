using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IGenericRepository<T>
    {
       
        Task<T> GetByIdAsync(string id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<string?> AddAsync(T entity);

        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
        
    }
}
