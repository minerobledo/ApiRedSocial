using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Domain.Entities.Notification;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    internal class FirebaseMessagingRepository : GenericRepository<NotificationEntity>, IFirebaseMessagingRepository
    {
        private static readonly Dictionary<string, Type> NotificationTypes = new()
            {
                { nameof(NotificationEntity), typeof(NotificationEntity) },
                { nameof(RequestFaceNotification), typeof(RequestFaceNotification) }
            };


        private readonly string _SenderIdFild = "SenderId";
        private readonly string _ReseptorIdFild = "SenderId";
        private readonly string _StatusFild = "Status";
        public FirebaseMessagingRepository(FirestoreDb firestoreDb, string collectionName = "Notifications") : base(firestoreDb, collectionName)
        {
           
        }

        public async Task<string?> SendNotificationAsync(string deviceToken)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = "Notificacion",
                    Body = "Tienes una notificacion Pendiente"
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High, // Importante para notificaciones flotantes
                    Notification = new AndroidNotification
                    {
                        ChannelId = "high_importance_channel", // Debe coincidir con el ID del canal en Flutter
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                        Icon = "ic_launcher", // Asegúrate de tener este ícono en res/mipmap
                    }



                }
            };
            try
            {
                // Envía la notificación
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return response; // Retorna el ID de la notificación enviada
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error al enviar la notificación: {ex.Message}");
                Console.WriteLine($"Detalle del error: {ex.InnerException?.Message}");
                Console.WriteLine($"Código de error: {ex.MessagingErrorCode}");

                // Verificamos si el error se debe a un token inválido
                if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                    ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    return null;
                }

                throw; // Re-lanzamos la excepción si es necesario manejarla en un nivel superior
            }
        }

        public async Task<string?> SendAndSaveNotification(NotificationEntity notificationEntity,List<string> devices)
        {
            try
            {
                var docRef = await _firestoreDb.Collection(_collectionName).AddAsync(notificationEntity);
                List<Task<string?>> task = new List<Task<string?>>();
                if(notificationEntity.Type == "gloval")
                {
                    await SendGlobalNotificationByTopicAsync("gloval");
                }
                else
                {
                    foreach(var device in devices)
                    {
                        task.Add(SendNotificationAsync(device));
                    }

                }
                await Task.WhenAll(task);
                return docRef.Collection(_collectionName).Document().Id!;

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            

        }
        public async Task<string> SendGlobalNotificationByTopicAsync(string topic)
        {
            var message = new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = "Notificacion",
                    Body = "Tienes una notificacion Pendiente"
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "important_notifications",
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                        Icon = "ic_launcher"
                    }
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Notificación global enviada con éxito: {response}");
                return response;
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error al enviar la notificación global: {ex.Message}");
                throw;
            }
        }

        public async Task<List<NotificationEntity>?> GetNotificationsByProfilId(string ProfileId)
        {
            if (string.IsNullOrWhiteSpace(ProfileId))
            {
                return null;
            }

            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("ProfileId", ProfileId);
                var snapshot = await query.GetSnapshotAsync();
                List<NotificationEntity> result = new List<NotificationEntity>();

                foreach (var notification in snapshot)
                {
                    string notificationType = notification.ContainsField("NotificationType")
                                              ? notification.GetValue<string>("NotificationType")
                                              : nameof(NotificationEntity);

                    if (NotificationTypes.TryGetValue(notificationType, out Type? type))
                    {
                        var method = typeof(DocumentSnapshot).GetMethod(nameof(DocumentSnapshot.ConvertTo))?.MakeGenericMethod(type);
                        if (method != null)
                        {
                            var entity = method.Invoke(notification, null) as NotificationEntity;
                            if (entity != null)
                            {
                                result.Add(entity);
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }
        public async Task<string?> GetStatusRequestFacePhotoIfExist(string senderId, string reseptorId)
        {
            if (string.IsNullOrWhiteSpace(senderId)|| string.IsNullOrWhiteSpace(reseptorId)) return "";
            var doc1 = new Dictionary<string, object>()
            {
                {"SenderId",senderId}
                
            };
            var doc2 = new Dictionary<string, object>()
            {
                {"ReseptorId",reseptorId }
            };
            try
            {
                var query = _firestoreDb.Collection("Notifications").Where
                    (
                    Filter.And(
                        Filter.EqualTo(_ReseptorIdFild,reseptorId),
                        Filter.EqualTo(_SenderIdFild,senderId)
                        )
                    )
                    .Limit(1).Select(_StatusFild);
                var result = await query.GetSnapshotAsync();
                if (result.Count == 1)
                {
                    
                    return result[0].ToString();
                }
                return "";
            }catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return null;
            }

        }
        public async Task<bool?> DeleteNotification(string notificationId)
        {
            if (string.IsNullOrWhiteSpace(notificationId)) return null;

            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(notificationId);
                var result = await docRef.DeleteAsync();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return false;
            }
        }
        public async Task<bool?> DeleteNotification(string notificationId, string profileID)
        {
            if (string.IsNullOrWhiteSpace(notificationId)) return null;

            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(notificationId);
                var snapshot = await docRef.GetSnapshotAsync();
                var not= snapshot.ConvertTo<NotificationEntity>();

                if(not.ProfileId == profileID) 
                {
                    await docRef.DeleteAsync();
                    return true;
                }
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }
        public async Task<bool?> ReadNotification(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "IsRead", true } });
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return false;
            }
        }
        public async Task<List<NotificationEntity>?> GetGlovalNotification()
        {
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).WhereEqualTo("ProfileId", "gloval").OrderByDescending("CreatedAt");
                var a =await docRef.GetSnapshotAsync();
                var list = new List<NotificationEntity>();
                foreach (var item in a)
                {
                    list.Add(item.ConvertTo<NotificationEntity>());
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }
    }
}
