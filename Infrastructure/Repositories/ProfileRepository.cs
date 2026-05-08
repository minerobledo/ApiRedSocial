using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using Geohash;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Transactions;
//using static Google.Cloud.Firestore.V1.StructuredQuery.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Infrastructure.Repositories
{
    internal class ProfileRepository : GenericRepository<Domain.Entities.Profile>, IProfileRepository
    {
        private readonly IMapper _mapper;
        private readonly Geohasher _geohasher;

        //Constructor
        public ProfileRepository(IMapper mapper, FirestoreDb firestoreDb, string collectionName = "Profiles") : base(firestoreDb, collectionName)
        {
            //establese la coneccion con la bace dedatos para esta instancia
            _mapper = mapper;
            _geohasher = new Geohasher();
        }

        public async Task<bool?> DeleteProfileById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var result = await docRef.DeleteAsync();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<string?> AddTransactionAsync(Google.Cloud.Firestore.Transaction transaction, Domain.Entities.Profile profile)
        {
            if (profile is null)
            {
                return null;
            }



            //generamos el token de padrino
            profile.TokenGodfather = await GenerateUniqueGodfatherTokenAsync();
            //generamos el token de login
            profile.TokenLogin = await GenerateUniqueTokenLoginAsync();
            //genramos el prefijo para las busquedas
            profile.NameProfilePrefixes = GeneratePrefixes(profile.NameProfile!);
            //infomacion adicional
            profile.Connected = false;
            profile.LocationActive = false;
            profile.Interest = "Nada en particular";

            var perfilRef = _firestoreDb.Collection(_collectionName).Document();

            transaction.Set(perfilRef, profile);

            return perfilRef.Id;
        }
        public async Task<Domain.Entities.Profile?> GetProfileByTokenAsync(string Token)
        {
            if (Token == null) return null;
            try
            {
                var profileDoc = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenLogin", Token);
                var snapshot = await profileDoc.GetSnapshotAsync();
                
                if (snapshot.Count == 0)
                {
                    return null; // No se encontró ningún perfil con ese token
                }

                var document = snapshot.Documents.First();

                return document.ConvertTo<Domain.Entities.Profile>();
            } catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return null;
            }


        }
        public async Task<Domain.Entities.Profile?> GetProfileByTokenGodfatherAsync(string Token)
        {
            if (Token == null) return null;
            try
            {
                var profileDoc = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenGodfather", Token);
                var snapshot = await profileDoc.GetSnapshotAsync();

                if (snapshot.Count == 0)
                {
                    return null; // No se encontró ningún perfil con ese token
                }

                var document = snapshot.Documents.First();

                return document.ConvertTo<Domain.Entities.Profile>();


            } catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return null;
            }


        }
        public async Task<Domain.Entities.Profile?> GetProfileByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            try
            {

                var profileDoc = _firestoreDb.Collection(_collectionName).WhereEqualTo("NameProfile", name);
                var snapshot = await profileDoc.GetSnapshotAsync();

                if (snapshot.Count == 0)
                {
                    return null; // No se encontró ningún perfil con ese token
                }

                var document = snapshot.Documents.First();

                return document.ConvertTo<Domain.Entities.Profile>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<Domain.Entities.Profile?> GetProfileByIdAsync(string profileId)
        {
            if (profileId == null) return null;
            var profileDoc = _firestoreDb.Collection(_collectionName).Document(profileId); //busca un Documento en la coleccion Profiles sergun el Id, DocumentReference es un puntero Al documento en Firestore, el documento real
            var snapshot = await profileDoc.GetSnapshotAsync();//Apartir de La referencia Hace una snapshot de la informacion real

            if (!snapshot.Exists)//Pregunta si existe el Documento
            {
                return null;
            }
            //lo devuleve convertido en modelo
            return snapshot.ConvertTo<Domain.Entities.Profile>();
        }
        public async Task<List<ProfileShortDto>?> GetProfileByFilterAsync(Dictionary<string,object> filter,DateTime? startAfterId = null)
        {
            
            try
            {
                Query query = _firestoreDb.Collection(_collectionName);

                // Agregar los filtros dinámicamente
                foreach (var filtro in filter)
                {
                    switch (filtro.Key)
                    {
                        case "NameProfilePrefixes":
                            // code block
                            query = query.WhereArrayContains("NameProfilePrefixes", filtro.Value.ToString());
                            break;
                        case "NumberPersonAuthenticate":
                            // code block
                            if (filtro.Value == null)
                            {
                                query = query.WhereEqualTo(filtro.Key, null);
                            }
                            else
                            {
                                var a = int.Parse(filtro.Value.ToString()!);
                                query = query.WhereLessThanOrEqualTo(filtro.Key,a);
                            }
                            break;
                        case "PaymentPending":
                            query = query.WhereEqualTo("DateLastPayment", null);
                            break;
                        case "GoingExpiredSubscription":
                            query = query.WhereGreaterThan("DateVencetPayment", DateTime.UtcNow);
                            query = query.WhereLessThanOrEqualTo("DateVencetPayment", DateTime.UtcNow.AddDays(10));
                            break;
                        case "ExpiredSubscription":
                            query = query.WhereLessThanOrEqualTo("DateVencetPayment", DateTime.UtcNow);
                            break;
                        default:
                            // code block
                            query = query.WhereEqualTo(filtro.Key, filtro.Value);
                            break;
                    }
                }

                // Ordenar por un campo conocido (por ejemplo "CreateAt" o "Id")
                query = query.OrderByDescending("EntryDate");

                // Agregar paginación si viene un ID del último documento
                if (startAfterId.HasValue)
                {
                    query= query.StartAfter(startAfterId);
                }
               

                // Limitar los resultados
                query = query.Limit(10);

                // Ejecutar la consulta
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                List<ProfileShortDto> ids = new List<ProfileShortDto>();
                foreach (var doc in snapshot.Documents)
                {
                    ids.Add(_mapper.Map<ProfileShortDto>( doc.ConvertTo<Domain.Entities.Profile>()));
                }

                return ids;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
        public async Task<List<Domain.Entities.Profile>?> GetProfileListByListName(List<string> name)
        {
            if (name.Count == 0 || name is null) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereIn("NameProfile", name);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count != 0)
                {
                    List<Domain.Entities.Profile> profiles = new List<Domain.Entities.Profile>();
                    foreach (var item in snapshot.Documents)
                    {
                        profiles.Add(item.ConvertTo<Domain.Entities.Profile>());
                    }
                    return profiles;
                }
                return null;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<ProfileShortDto>?> GetProfileShortListByListId(List<string> ids)
        {
            if (ids == null || ids.Count == 0) return null;
            try
            {
                CollectionReference collection = _firestoreDb.Collection(_collectionName);
                List<Task<QuerySnapshot>> tareas = new List<Task<QuerySnapshot>>();
                List<ProfileShortDto> resultados = new List<ProfileShortDto>();

                for (int i = 0; i < ids.Count; i += 30)
                {
                    List<string> batch = ids.GetRange(i, Math.Min(30, ids.Count - i));

                    Query query = collection
                        .WhereIn(FieldPath.DocumentId, batch);
                        //.Select("NameProfile", "ProfilePhoto", "User1Province", "User2Province"); // Seleccionamos los campos necesarios

                    tareas.Add(query.GetSnapshotAsync());
                }

                QuerySnapshot[] snapshots = await Task.WhenAll(tareas);

                foreach (var snapshot in snapshots)
                {
                    foreach (var doc in snapshot.Documents)
                    {
                        var perfil = doc.ConvertTo<Domain .Entities.Profile>();

                        perfil.Id = doc.Id; // Como el ID no se selecciona, lo asignamos manualmente.
                        
                        resultados.Add(_mapper.Map<ProfileShortDto>(perfil));
                    }
                }

                return resultados;

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<DeviceToken>?> GetDeviceTokenAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef =  _firestoreDb.Collection(_collectionName).Document(id);
                var sanpshot = await docRef.GetSnapshotAsync();
                List<DeviceToken> result =new List<DeviceToken>();
                if (sanpshot.Exists)
                {
                    var profil = sanpshot.ConvertTo<Domain.Entities.Profile>();
                    if(profil.User1DeviceTokens!= null)
                    {
                       result  = result.Concat(profil.User1DeviceTokens!).ToList();
                    }
                    if(profil.User2DeviceTokens!= null)
                    {
                        result = result.Concat(profil.User2DeviceTokens!).ToList();
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> UpdateIntesrestById(string id, string interest)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(interest)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "Interest", interest } });
                return true;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        
        public async Task<bool?> UpdateGodFatherResponce(string id, bool responce)
        {
            if(string.IsNullOrEmpty(id) ) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    {"PadrinoHaRespondido",responce}
                });
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> UpdateSponsoredNumbers(string id, int number)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    {"SponsoredNumbers",number}
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> SetTrustedDevice(string deviceID, string marca, string modelo, int? user,string profileID)
        {
            if (string.IsNullOrEmpty(marca) || string.IsNullOrEmpty(deviceID) || string.IsNullOrEmpty(modelo) || string.IsNullOrEmpty(profileID) || !user.HasValue) return null;
            try 
            {
                var countQuery = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + user).Count();
                var snapshot= await countQuery.GetSnapshotAsync();
                if(snapshot.Count == 10)
                {
                    return false;
                }



                var docRef = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser"+user).Document();
                var a = new TrustedDevice { Id = deviceID, Marca = marca, Modelo = modelo, CreateAt = DateTime.UtcNow, LastLoginAt = DateTime.UtcNow };
                await docRef.SetAsync(a);
                return true;
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); return null; }
        }
        public async Task<int?> GetTrustedDeviceByDeviceId(string deviceID, string profileID)
        {
            if (string.IsNullOrEmpty(deviceID) || string.IsNullOrEmpty(profileID) ) return null;
            try
            {
                var query1 = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + 1).WhereEqualTo("Id",deviceID);
                var query2 = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + 2).WhereEqualTo("Id", deviceID);
                var snap1 = await query1.GetSnapshotAsync();
                var snap2 = await query2.GetSnapshotAsync();
                if (snap1.Count != 0 && snap2.Count != 0) return null;
                if (snap1.Count == 1 && snap2.Count == 0)
                {
                    await _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + 1).Document(snap1.First().Id).UpdateAsync(new Dictionary<string, object> { { "LastLoginAt", DateTime.UtcNow } });
                    return 1;
                }
                if (snap1.Count == 0 && snap2.Count == 1)
                { 
                    await _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + 2).Document(snap1.First().Id).UpdateAsync(new Dictionary<string, object> { { "LastLoginAt", DateTime.UtcNow } });
                    return 2;
                }
            
                return null;
            }

            catch (Exception ex) { Console.WriteLine(ex.ToString()); return null; }
        }
        public async Task<List<TrustedDevice>?> GetAllTrustedDevice(string profileID, int user)
        {
            if (string.IsNullOrEmpty(profileID)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + user).Limit(10);
                var snap = await query.GetSnapshotAsync();
                var list = new List<TrustedDevice>();
                if (snap.Count != 0)
                {
                    foreach (var tr in snap)
                    {
                        list.Add(tr.ConvertTo<TrustedDevice>());
                    }

                }
                return list;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); return null; }
        }

        public async Task<bool?> DeleteTrustedDevice(string documentID, string profileID, int user)
        {
            if (string.IsNullOrEmpty(documentID) || string.IsNullOrEmpty(profileID)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Document(profileID).Collection("TrsutedDeviceUser" + user).Document(documentID);
                var snap = await query.DeleteAsync();
                
                return true;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString()); return false; 
            }
        }
        public async Task<List<Domain.Entities.Profile>?> SerchProfile(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            try
            {
                CollectionReference usersRef = _firestoreDb.Collection(_collectionName);
                Query snapshotQuery = usersRef.WhereArrayContains("NameProfilePrefixes", name).Limit(20);
                QuerySnapshot snapshot = await snapshotQuery.GetSnapshotAsync();
                

                var list = new List<Domain.Entities.Profile>();
                foreach (var item in snapshot.Documents)
                {
                    list.Add(item.ConvertTo<Domain.Entities.Profile>());
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task UpdateProfileAsync(Domain.Entities.Profile profile)
        {
            DocumentReference profileDoc = _firestoreDb.Collection(_collectionName).Document(profile.Id);
            await profileDoc.SetAsync(profile, SetOptions.Overwrite);
        }
        public async Task<bool?> UpdateGeoPoint (GeoPoint geoPoint,string id, int user)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(id) ) { return  null; }
            try
            {
                var dccRef = _firestoreDb.Collection(_collectionName).Document(id);
                
                var a = new Dictionary<string, object>() 
                {
                    { "User" + user + "GeoPoint", geoPoint }
                };
                
                var result = await dccRef.UpdateAsync(a);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

       
        public async Task<List<ProfileShortDto>> GeProfileInMaps(Dictionary<string, object>? filter, double Rkm, double lat, double lng)
        {
            var httpClient = new HttpClient();

            // 🔐 1. Obtener el ID token del usuario autenticado
            string idToken = await ObtenerAccessTokenAsync(); // Implementar esta función o pasarlo como parámetro
            //Console.WriteLine(idToken);
            if (string.IsNullOrEmpty(idToken))
                throw new Exception("ID Token no disponible");

            // 🌐 2. URL de tu función en Firebase (ajustala si es regional diferente)
            var url = "https://buscarperfilescercanos-cxjp4uxzrq-uc.a.run.app";

            // 📦 3. Construir payload (body)
            var requestBody = new
            {
                center = new
                {
                    latitude = lat,
                    longitude = lng
                },
                radiusInKm = Rkm,
                filtros = filter
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // 🧾 4. Agregar header de autorización
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            // 🚀 5. Enviar POST
            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Response: {response.StatusCode}");
                throw new Exception($"Error al llamar Firebase Function: {response.StatusCode}");
            }


            // 📥 6. Leer respuesta
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var perfiles = JsonSerializer.Deserialize<List<ProfileShortDto>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return perfiles ?? new List<ProfileShortDto>();
        }
        
        public async Task<bool?> BanProfile(string id,DateTime unBanDate, string Reason)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(Reason)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    {"Ban",true },
                    {"UnBanDate",unBanDate},
                    {"BanReason",Reason }
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> UnBan(string id, DateTime unBanDate, string Reason)
        {
            if (string.IsNullOrEmpty(id) ) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object?>
                {
                    {"Ban",false },
                    {"UnBanDate",null},
                    {"BanReason",null }
                }); 
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public async Task<bool?> RemubeDays(string id, DateTime dateTime)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
               
                
                await docRef.UpdateAsync(new Dictionary<string, object?>
                {
                    {"DateVencetPayment",dateTime }                
                });
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool DeleteTransaccionAsync(Google.Cloud.Firestore.Transaction transaction, string profileId)
        {
            if (profileId is null)
            {
                return false;
            }

            var perfilRef = _firestoreDb.Collection(_collectionName).Document(profileId);

            transaction.Delete(perfilRef);

            return true;
        }

        public async Task<bool?> ExistProfileByNameProfileAsync(string nameProfile)
        {
            if (string.IsNullOrEmpty(nameProfile)) return null;
            var query = _firestoreDb.Collection(_collectionName)
                                    .WhereEqualTo("NameProfile", nameProfile)
                                    .Limit(1); // Solo necesitamos saber si existe uno
            try
            {
                var snapshot = await query.GetSnapshotAsync();

                return snapshot.Count > 0; // Si hay al menos un documento, el perfil existe
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> ExistProfileByTokenGodfather(string Token)
        {
            if (Token == null) return false;
            var documentReference = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenGodfather", Token);
            var snapshot = await documentReference.GetSnapshotAsync();
            if (snapshot.Count == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> ExistProfileByLoginToken(string Token)
        {
            var profileDoc = _firestoreDb.Collection(_collectionName).WhereEqualTo("Token", Token);
            var snapshot = await profileDoc.GetSnapshotAsync();

            if (snapshot.Count == 1)
            {
                return true; // No se encontró ningún perfil con ese token
            }
            return false;
        }
        public async Task<bool?> ExistProfileByEmailAsync(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email)) return null;
            var query = _firestoreDb.Collection(_collectionName)
                                    .Where(Filter.Or(
                                        Filter.EqualTo("User1Email", Email),
                                        Filter.EqualTo("User2Email", Email)
                                                    ))
                                    .Limit(1); // Solo necesitamos saber si existe uno
            try
            {
                var snapshot = await query.GetSnapshotAsync();
                if (!snapshot.Any()) return false;
                return true; // Si hay al menos un documento, el perfil existe
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> ExistProfileByPhoneAsync(string Phone)
        {
            if (string.IsNullOrEmpty(Phone)) return null;
            var query = _firestoreDb.Collection(_collectionName)
                                    .Where(Filter.Or(
                                        Filter.EqualTo("Usuario2PhoneNumber ", Phone),
                                        Filter.EqualTo("Usuario2PhoneNumber ", Phone)
                                                    ))
                                    .Limit(1); // Solo necesitamos saber si existe uno
            try
            {
                var snapshot = await query.GetSnapshotAsync();

                return snapshot.Count > 0; // Si hay al menos un documento, el perfil existe
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        public async Task ConectedOnOff(Domain.Entities.Profile profile, bool state)
        {
            var documentReference = _firestoreDb.Collection(_collectionName).Document(profile.Id);

            await documentReference.UpdateAsync(new Dictionary<string, object>
            {
                { "Connected", state }
            });



        }
       
        public async Task AddOrUpdateDeviceTokenAsync(Domain.Entities.Profile profile,int? user,DeviceToken deviceToken)
        {
            var profilRef = _firestoreDb.Collection(_collectionName).Document(profile.Id);
            if(user == 1)
            {
                bool a = true; 
                if(profile.User1DeviceTokens == null) profile.User1DeviceTokens = new List<DeviceToken>();
                foreach (var device in profile.User1DeviceTokens)
                {
                    if(device.Token == deviceToken.Token)
                    {
                        device.Token = deviceToken.Token;
                        device.LastUpdated = DateTime.UtcNow;
                        a =false;
                    }
                }

                if(a) profile.User1DeviceTokens.Add(deviceToken);

            }
            if (user == 2)
            {
                bool a = true;
                if (profile.User2DeviceTokens == null) profile.User2DeviceTokens = new List<DeviceToken>();
                foreach (var device in profile.User2DeviceTokens)
                {
                    if (device.Token == deviceToken.Token)
                    {
                        device.Token = deviceToken.Token;
                        device.LastUpdated = DateTime.UtcNow;
                        a = false;
                    }
                }

                if (a) profile.User2DeviceTokens.Add(deviceToken);
            }
            try
            {

                await profilRef.SetAsync(profile);
            }catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
            }
        }
        public async Task RemuveDeviceTokenAsync(Domain.Entities.Profile profile, int? user, DeviceToken deviceToken)
        {
            var profilRef = _firestoreDb.Collection(_collectionName).Document(profile.Id);

            if (user == 1)
            {
                bool a = true;
                foreach (var device in profile.User1DeviceTokens)
                {
                    if (device.Token == deviceToken.Token)
                    {
                        device.Token = deviceToken.Token;
                        device.LastUpdated = DateTime.UtcNow;
                        a = false;
                    }
                }

                if (a) profile.User1DeviceTokens.Remove(deviceToken);

            }
            if (user == 2)
            {
                bool a = true;
                foreach (var device in profile.User1DeviceTokens)
                {
                    if (device.Token == deviceToken.Token)
                    {
                        device.Token = deviceToken.Token;
                        device.LastUpdated = DateTime.UtcNow;
                        a = false;
                    }
                }

                if (a) profile.User1DeviceTokens.Remove(deviceToken);
            }
            try
            {

                await profilRef.SetAsync(profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task<bool?> UpdateProfileById(Domain.Entities.Profile profile)
        {
            if (string.IsNullOrEmpty(profile.Id)) { return null; }
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(profile.Id);
                await docRef.SetAsync(profile);
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        
        
        }

        public async Task<bool?> ChangesAcesLimit(string id, bool state)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docref = _firestoreDb.Collection(_collectionName).Document(id);
                var result =await docref.UpdateAsync(new Dictionary<string, object>
                {
                    {"AccessLimit",state }
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> AddDeys(string id, int  days)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docref = _firestoreDb.Collection(_collectionName).Document(id);
                var result = await docref.UpdateAsync(new Dictionary<string, object>
                {
                    {"DateLastPayment", DateTime.UtcNow },
                    {"DateVencetPayment",DateTime.UtcNow.AddDays(days) },
                    {"AccessLimit", false }
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool?> VerifyProfile(string id , string selfId, bool admin)
        {
            if(string.IsNullOrEmpty(id)|| string.IsNullOrEmpty(selfId)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(selfId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                int count = snapshot.ContainsField("miArrayCount") ? snapshot.GetValue<int>("miArrayCount") : 0;
                Dictionary<string, object> update;
                if (admin)
                {
                    update = new()
                    {
                        { "ListProfileAuthenticate", FieldValue.ArrayUnion(id) },
                        {"NumberPersonAuthenticate", count+5}
                    };
                }
                else
                {
                    update = new()
                    {
                        { "ListProfileAuthenticate", FieldValue.ArrayUnion(id) },
                        {"NumberPersonAuthenticate", count+1}
                    };
                }
                await docRef.UpdateAsync(update);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        //esta para quitar
        public override async Task<string?> AddAsync(Domain.Entities.Profile profile)
        {
            return await base.ExecuteWithRetryAsync(async () =>
            {
                profile.TokenGodfather = await GenerateUniqueGodfatherTokenAsync();
                profile.TokenLogin = await GenerateUniqueTokenLoginAsync();
                profile.NameProfilePrefixes = GeneratePrefixes(profile.NameProfile!);
                profile.Connected = false;
                profile.LocationActive = false;
                profile.Interest = "Nada en particular";
                DateTime fechaActual = DateTime.Now;
                
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                Google.Cloud.Firestore.WriteResult result = await docRef.SetAsync(profile);
                if (result is null)
                    return null;

                return docRef.Collection(_collectionName).Document().Id;

            });

           

        }
        public async Task<List<Domain.Entities.Profile>> GetProfilesAsync()
        {
            Query profileQuery = _firestoreDb.Collection(_collectionName);
            QuerySnapshot snapshots = await profileQuery.GetSnapshotAsync();

            List<Domain.Entities.Profile> profiles = new List<Domain.Entities.Profile>();
            foreach (var document in snapshots.Documents)
            {
                profiles.Add(document.ConvertTo<Domain.Entities.Profile>());
            }

            return profiles;
        }


        //funciones internas

        public async Task<string> ObtenerAccessTokenAsync()
        {
            string[] scopes = { "https://www.googleapis.com/auth/cloud-platform" };

            GoogleCredential credential;

            // Intenta leer desde variables de entorno
            var envType = Environment.GetEnvironmentVariable("type");
            if (!string.IsNullOrEmpty(envType))
            {
                var credentialsJson = $@"
                    {{
                        ""type"": ""{Environment.GetEnvironmentVariable("type")}"",
                        ""project_id"": ""{Environment.GetEnvironmentVariable("project_id")}"",
                        ""private_key_id"": ""{Environment.GetEnvironmentVariable("private_key_id")}"",
                        ""private_key"": ""{Environment.GetEnvironmentVariable("private_key")?.Replace("\\n", "\n")}"",
                        ""client_email"": ""{Environment.GetEnvironmentVariable("client_email")}"",
                        ""client_id"": ""{Environment.GetEnvironmentVariable("FIREBASE_CLIENT_ID")}"",
                        ""auth_uri"": ""{Environment.GetEnvironmentVariable("auth_uri")}"",
                        ""token_uri"": ""{Environment.GetEnvironmentVariable("token_uri")}"",
                        ""auth_provider_x509_cert_url"": ""{Environment.GetEnvironmentVariable("auth_provider_x509_cert_url")}"",
                        ""client_x509_cert_url"": ""{Environment.GetEnvironmentVariable("client_x509_cert_url")}"",
                        ""universe_domain"": ""{Environment.GetEnvironmentVariable("universe_domain")}""
                    }}";

                credential = GoogleCredential.FromJson(credentialsJson);
            }
            else
            {
                Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                // Fallback: leer desde archivo
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-credentials.json");
                credential = GoogleCredential.FromFile(path);
            }

            var token = await credential
                .CreateScoped(scopes)
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync();

            return token;
        }

        private List<string> GeneratePrefixes(string nickname)
        {
            var prefixes = new List<string>();
            nickname = nickname.ToLower();

            for (int i = 1; i <= nickname.Length; i++)
            {
                prefixes.Add(nickname.Substring(0, i));
            }

            return prefixes;
        }

        private async Task<string> GenerateUniqueGodfatherTokenAsync()
        {
            string code;
            bool isUnique;

            do
            {
                code = GenerateSixDigitCode();
                isUnique = await IsUniqueTokenGodfatherAsync(code);
            }
            while (!isUnique);

            return code;
        }
        private async Task<string> GenerateUniqueTokenLoginAsync()
        {
            string code;
            bool isUnique;

            do
            {
                code = GenerateSixDigitCode();
                isUnique = await IsUniqueTokenLoginAsync(code);
            }
            while (!isUnique);

            return code;
        }
        private static string GenerateSixDigitCode()
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000);
            return code.ToString("D6");
        }
        private async Task<bool> IsUniqueTokenGodfatherAsync(string code)
        {

            var codeRepetTokenRegis = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenGodfather", code);
            var ListCodeTokenRegis = await codeRepetTokenRegis.GetSnapshotAsync();

            if (!ListCodeTokenRegis.Any())
                return true;

            return false;
        }
        private async Task<bool> IsUniqueTokenLoginAsync(string code)
        {
            var CodeRepetTokenLogin = _firestoreDb.Collection(_collectionName).WhereEqualTo("TokenLogin", code);

            var listCodesTokenLogin = await CodeRepetTokenLogin.GetSnapshotAsync();
            if (!listCodesTokenLogin.Any())
                return true;

            return false;

        }
    }
}
