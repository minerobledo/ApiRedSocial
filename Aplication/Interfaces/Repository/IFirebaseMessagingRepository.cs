using Domain.Entities.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IFirebaseMessagingRepository
    {
        Task<string?> SendNotificationAsync(string deviceToken);
        Task<string?> SendAndSaveNotification(NotificationEntity notificationEntity, List<string> devices);
        Task<string> SendGlobalNotificationByTopicAsync(string topic);
        Task<string?> GetStatusRequestFacePhotoIfExist(string senderId, string reseptorId);
        Task<List<NotificationEntity>?> GetNotificationsByProfilId(string ProfileId);
        Task<bool?> DeleteNotification(string notificationId);
        Task<bool?> DeleteNotification(string notificationId,string profileID);
        Task<List<NotificationEntity>?> GetGlovalNotification();
        Task<bool?> ReadNotification(string id);

    }
}
