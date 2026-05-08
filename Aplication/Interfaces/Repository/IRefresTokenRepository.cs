using Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IRefresTokenRepository
    {
        Task<bool?> AddDocumentAsync(string userId, string profileId, string refreshToken, string jwt, int? user = null);
        Task<RefreshToken?> ExistRefreshtoken(string refresh);

        Task<string?> GetRefresTokenDocumentIdIfExist(string id, int? user = null);
        Task<bool?> UpdateAsync(string RefreshTokenValue, string JwtToken, string documentID);
        Task DeleteRefeshtoken(string id);

    }
}
