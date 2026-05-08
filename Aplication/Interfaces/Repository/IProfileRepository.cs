using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Domain.Entities;
using Google.Cloud.Firestore;
using Aplication.DTO.OutputDto.Profile;
using Domain.Entities.Notification;


namespace Aplication.Interfaces.Repository
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {

        Task<string?> AddTransactionAsync(Transaction transaction, Domain.Entities.Profile profile);
        Task<bool?> BanProfile(string id, DateTime unBanDate, string Reason);
        Task<Profile?> GetProfileByTokenAsync(string Token);
        Task<Profile?> GetProfileByTokenGodfatherAsync(string Token);
        Task<Domain.Entities.Profile?> GetProfileByIdAsync(string profileId);
        Task<List<Domain.Entities.Profile>?> GetProfileListByListName(List<string> name);
        Task<List<Domain.Entities.Profile>?> SerchProfile(string name);
        Task<Domain.Entities.Profile?> GetProfileByNameAsync(string name);
        Task<List<ProfileShortDto>?> GetProfileShortListByListId(List<string> ids);
        Task<List<DeviceToken>?> GetDeviceTokenAsync(string id);
        Task ConectedOnOff(Profile profile,bool state);
        Task<List<Profile>> GetProfilesAsync();
        Task UpdateProfileAsync(Profile profile);
        Task<bool?> UpdateIntesrestById(string id, string interest);
        Task<bool?> UpdateGodFatherResponce(string id, bool responce);
        Task AddOrUpdateDeviceTokenAsync(Domain.Entities.Profile profile, int? user, DeviceToken deviceToken);
        Task RemuveDeviceTokenAsync(Domain.Entities.Profile profile, int? user, DeviceToken deviceToken);
        bool DeleteTransaccionAsync(Transaction transaction, string profileId);
        Task<bool?> ExistProfileByNameProfileAsync(string profileName);
        Task<bool> ExistProfileByLoginToken(string Token);
        Task<bool> ExistProfileByTokenGodfather(string Token);
        Task<bool?> ExistProfileByEmailAsync(string Email);
        Task<bool?> ExistProfileByPhoneAsync(string Phone);
        Task<bool?> UpdateGeoPoint(GeoPoint geoPoint, string id, int user);
        Task<bool?> ChangesAcesLimit(string id, bool state);
        Task<bool?> AddDeys(string id, int days);
        Task<bool?> RemubeDays(string id, DateTime dateTime);
        Task<List<ProfileShortDto>?> GetProfileByFilterAsync(Dictionary<string, object> filter, DateTime? startAfterId = null);
        Task<List<ProfileShortDto>> GeProfileInMaps(Dictionary<string, object>? filter, double Rkm, double lat, double lng);
        Task<bool?> UpdateProfileById(Domain.Entities.Profile profile);
        Task<bool?> SetTrustedDevice(string deviceID, string marca, string modelo, int? user, string profileID);
        Task<int?> GetTrustedDeviceByDeviceId(string deviceID, string profileID);
        Task<List<TrustedDevice>?> GetAllTrustedDevice(string profileID, int user);
        Task<bool?> DeleteTrustedDevice(string documentID, string profileID, int user);
        Task<bool?> VerifyProfile(string id, string selfId,bool admin);

        //Task<List<Dictionary<string, object>>?> ObtenerPerfilesPorNombreYProvinciaAsync(string provincia);

    }
}
