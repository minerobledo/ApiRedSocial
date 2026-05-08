
using Aplication.Interfaces.Repository;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class FriendshipRepository : GenericRepository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(FirestoreDb firestoreDb, string collectionName = "Frienships") : base(firestoreDb, collectionName)
        {

        }
        
        public async Task<Friendship?> GetFriendshipAsinc(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var snapchot = await docRef.GetSnapshotAsync();
                if (snapchot.Exists) {
                    return snapchot.ConvertTo<Friendship>();
                }
                return null;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        public async Task<Friendship?> GetFriendshipAsinc(string id1, string id2)
        {
           
            if (string.IsNullOrWhiteSpace(id1) || string.IsNullOrWhiteSpace(id2)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Where(
                    Filter.Or
                    (
                        Filter.And(Filter.EqualTo("Friend1Id", id1), Filter.EqualTo("Friend2Id", id2)),
                        Filter.And(Filter.EqualTo("Friend2Id", id1), Filter.EqualTo("Friend1Id", id2))
                    )
                   
                    )
                    .Limit(1);
                var querysnapshot = await docRef.GetSnapshotAsync();
                if (querysnapshot.Count != 0)
                {
                    return querysnapshot[0].ConvertTo<Friendship>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public string? AddFrienshipTransaction(Transaction transaction, string profilIdSender, string profilIDResiver, string nameSender, string nameReseptor)
        {
            if (string.IsNullOrWhiteSpace(profilIdSender) || string.IsNullOrWhiteSpace(profilIDResiver)) return null;
            var friendship = new Friendship()
            {
                Friend1Id = profilIDResiver,
                Friend2Id = profilIdSender,
                Friend1ProfileName = nameReseptor,
                Friend2ProfileName = nameSender,
                CreateAt = DateTime.UtcNow,
                Status = "pending"
            };
            
            try
            {
                var friendshipRef = _firestoreDb.Collection(_collectionName).Document();
                transaction.Set(friendshipRef, friendship);
                return friendshipRef.Id!;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<List<Friendship>?> GetAllFriendshipByProfilIdAsinc(string id)
        {

            if (string.IsNullOrWhiteSpace(id)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Where(
                    Filter.Or(
                        Filter.And(Filter.EqualTo("Friend1Id", id), Filter.NotEqualTo("Friend2Id", id)),
                        Filter.And(Filter.EqualTo("Friend2Id", id), Filter.NotEqualTo("Friend1Id", id))
                        ));
                var snapshot = await query.GetSnapshotAsync();
                List<Friendship> friendships = new List<Friendship>();
                if (snapshot.Any())
                {
                    foreach (var item in snapshot.Documents)
                    {
                        friendships.Add(item.ConvertTo<Friendship>());
                    }
                }
                return friendships;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<List<string>?> GetAllFriendsIDpByProfilIdAsinc(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId)) return null;

            try
            {
                var amistades1Query = _firestoreDb.Collection(_collectionName).Where
                    (
                            Filter.And(Filter.EqualTo("Friend1Id", profileId), Filter.NotEqualTo("Friend2Id", profileId))
                    );
                var amistades2Query = _firestoreDb.Collection(_collectionName).Where
                   (
                           Filter.And(Filter.EqualTo("Friend2Id", profileId), Filter.NotEqualTo("Friend1Id", profileId))
                   );
                
                var amistadesSnapshot = await amistades1Query.GetSnapshotAsync();
                var amistadesSnapshot2 = await amistades2Query.GetSnapshotAsync();
                
                var FriendsIds = new List<string>();
                if (amistadesSnapshot.Any())
                {
                    foreach(var item in amistadesSnapshot.Documents)
                    {
                        var friendships = item.ConvertTo<Friendship>();
                        if(profileId != friendships.Friend1Id)
                        {
                            FriendsIds.Add(friendships.Friend1Id);
                        }
                        if (profileId != friendships.Friend2Id)
                        {
                            FriendsIds.Add(friendships.Friend2Id);
                        }
                    }
                }
                if (amistadesSnapshot2.Any())
                {
                    foreach (var item in amistadesSnapshot2.Documents)
                    {
                        var friendships = item.ConvertTo<Friendship>();
                        if (profileId != friendships.Friend1Id)
                        {
                            FriendsIds.Add(friendships.Friend1Id);
                        }
                        if (profileId != friendships.Friend2Id)
                        {
                            FriendsIds.Add(friendships.Friend2Id);
                        }
                    }
                }
                return FriendsIds.Count >=1 ? FriendsIds : null;

                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener amigos: {ex.Message}");
                return null;
            }
        }

        public async Task<bool?> ChangeStatusFrienship(string ID, bool status)
        {

            if (string.IsNullOrWhiteSpace(ID)) { return null; }
            var docRef = _firestoreDb.Collection(_collectionName).Document(ID);
            try
            {
                var snapchot = await docRef.GetSnapshotAsync();
                if (snapchot.Exists)
                {
                    var friendship = snapchot.ConvertTo<Friendship>();
                    if (status)
                    {
                        friendship.Status = "accepted";
                    }
                    else
                    {
                        friendship.Status = "rejected";
                    }
                    
                    await docRef.SetAsync(friendship);
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            
        }

        public async Task<bool?> ExistFrienship(string Id1, string Id2)
        {
            if (string.IsNullOrWhiteSpace(Id1) || string.IsNullOrWhiteSpace(Id2)) return null;

            try
            {
                // Usamos 'WhereArrayContainsAny' para verificar si 'Friends' contiene al menos uno de los dos valores
                var query = _firestoreDb.Collection(_collectionName).Where(
                    Filter.Or(
                        Filter.And(Filter.EqualTo("Friend1Id", Id1), Filter.EqualTo("Friend2Id", Id2)),
                        Filter.And(Filter.EqualTo("Friend2Id", Id1), Filter.EqualTo("Friend1Id", Id2))
                        )
                );
                var snapshot = await query.GetSnapshotAsync();

                // Verificamos si encontramos un documento
                if (snapshot.Any())
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}
