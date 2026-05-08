using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IAdminRepocitory
    {
        Task<Admin?> GetAdminById(string id);
        Task<bool?> CreateAdmin(string email, string password, string name, string Lname);
        Task<bool?> ExistAdminByID(string id);
        Task<Admin?> GetAdminByTokenLogin(string token);

    }
}
