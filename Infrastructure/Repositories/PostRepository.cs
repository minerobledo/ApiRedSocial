using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using Domain.Entities.Notification;
using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class PostRepository : GenericRepository<NotificationEntity>, IPostRepository
    {
        private const string _publicPostType = "public";
        private const string _fiendsPostType = "friends";


        public PostRepository(FirestoreDb firestoreDb, string collectionName = "Posts") : base(firestoreDb, collectionName)
        {


        }
        public async Task<Post?> GetPostById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                   return snapshot.ConvertTo<Post>();
                }
                return null;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<bool?> UpdatePostById(string id, Post post)
        {
            if (string.IsNullOrEmpty(id) || post == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var result = await docRef.SetAsync(post);
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<string?> UploadPost(Post post)
        {
            if (post == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                var result = await docRef.SetAsync(post);
                if (result != null)
                {
                    return docRef.Id;
                }
                return "";
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool?> DeletePostById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var result = await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool?> DeletePostWithIpPublisher(string postId, string publisherId)
        {
            if (string.IsNullOrWhiteSpace(publisherId) || string.IsNullOrWhiteSpace(postId)) return null;
            try
            {
                var docref = _firestoreDb.Collection(_collectionName).Document(postId);
                var snapshot = await docref.GetSnapshotAsync();
                if (snapshot == null) return null;
                var post = snapshot.ConvertTo<Post>();
                if(post.IdPublisher == publisherId)
                {
                    var result = await docref.DeleteAsync();
                    return true;
                }
                return false;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<Post>?> GetLastPublicPostPaginated(System.DateTime? dateTime)
        {
            
            CollectionReference postsRef = _firestoreDb.Collection(_collectionName);

            // Consulta base: Solo posts públicos, ordenados por fecha descendente
            Query query = postsRef
                .WhereEqualTo("PostType", _publicPostType)
                .WhereEqualTo("Status", "accepted");
                //.OrderByDescending("CreateAt")
                //.Limit(10);

            // Si tenemos un último DateTime de la página anterior, usamos StartAfter para paginar
            if (dateTime != null)
            {
                query = query.StartAfter(Timestamp.FromDateTime(dateTime.Value.ToUniversalTime()));
            }

            try { 
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                List<Post> posts = new List<Post>();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        Post post = doc.ConvertTo<Post>();
                        posts.Add(post);
                    }
                }
                return posts;

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<Post>?> GetLastedFriendsPostPaginated(System.DateTime? dateTime, List<string> ids)
        {
            if(ids == null) return null;

            CollectionReference postsRef = _firestoreDb.Collection("Posts");
            List<Post> allPosts = new List<Post>();

            try
            {
                // Dividir la lista de amigos en grupos de 10 (por la limitación de Firestore)
                foreach (var batch in ids.Chunk(10))
                {
                    Query query = _firestoreDb.Collection(_collectionName)
                        .WhereIn("IdPublisher", batch.ToList())  // Filtra por amigos
                        .WhereEqualTo("PostType", _fiendsPostType)
                        .WhereEqualTo("Status", "accepted")       // Solo posts aceptados
                        .OrderByDescending("CreateAt")           // Más recientes primero
                        .Limit(10);
                    if (dateTime != null)
                    {
                        query = query.StartAfter(Timestamp.FromDateTime(dateTime.Value.ToUniversalTime()));
                    }
                    QuerySnapshot snapshot = await query.GetSnapshotAsync();

                    foreach (DocumentSnapshot doc in snapshot.Documents)
                    {
                        if (doc.Exists)
                        {
                            Post post = doc.ConvertTo<Post>();
                            allPosts.Add(post);
                        }
                    }
                }

                // Ordenar y tomar solo los 10 posts más recientes
                return allPosts.OrderByDescending(p => p.CreateAt).Take(10).ToList();
                
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task<List<string>?> GedtALFacePostFromProfileId(string profileId)
        {
            if (string.IsNullOrEmpty(profileId)) return null; // 🔹 CORREGIDO

            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    .WhereEqualTo("IdPublisher", profileId) // Filtra por ProfileId
                    .WhereEqualTo("IsFace", true);       // Filtra por Type
                                            // 🔹 Solo trae el campo "Id"

                // Ejecutar la consulta
                var snapshot = await query.GetSnapshotAsync();

                var a = new List<string>();
                if (snapshot.Count > 0)
                {
                    // Extraer solo los valores del campo "Id" en los documentos
                    foreach(DocumentSnapshot doc in snapshot.Documents)
                    {
                        var post = doc.ConvertTo<Post>();
                        a.Add(post.Id);
                    }
                }

                return a;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<List<Post>?> GetSelfProfilePosts(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("IdPublisher",id);
                var snapshot = await query.GetSnapshotAsync();
                var list = new List<Post>();
                if (snapshot.Count >0)
                {
                    foreach (var item in snapshot)
                    {
                        list.Add(item.ConvertTo<Post>());
                    }
                }
                return list;
                

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<Post>?> GetFriendProfilePosts(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Where
                    (
                    Filter.And
                        (
                            Filter.EqualTo("IdPublisher", id),
                            Filter.EqualTo("IsFace", false),
                            Filter.Or(
                            Filter.EqualTo("PostType", _publicPostType),
                            Filter.EqualTo("PostType", _fiendsPostType)),
                            Filter.EqualTo("Status","accepted")

                        )
                    );
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count > 0)
                {
                    var list = new List<Post>();
                    foreach (var item in snapshot)
                    {
                        list.Add(item.ConvertTo<Post>());
                    }
                    return list;
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<Post>?> GetAceptedPublicProfilePosts(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Where
                    (
                        Filter.And
                        (
                            Filter.EqualTo("IdPublisher", id),
                            Filter.EqualTo("PostType", _publicPostType),
                            Filter.EqualTo("Status", "accepted")
                        )
                    );
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count > 0)
                {
                    var list = new List<Post>();
                    foreach (var item in snapshot)
                    {
                        list.Add(item.ConvertTo<Post>());
                    }
                    return list;
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<Post>?> GetPostByList(List<string> ids)
        {
            if(ids == null) return null;
            try
            {
                var batches = ids.Select((id, index) => new { id, index })
                                    .GroupBy(x => x.index / 30)
                                    .Select(g => g.Select(x => x.id).ToList())
                                    .ToList();

                // 🔹 Ejecutar todas las consultas en paralelo
                var tasks = batches.Select(async batch =>
                {
                    var query = _firestoreDb.Collection(_collectionName).WhereIn(FieldPath.DocumentId, batch);
                    return await query.GetSnapshotAsync();
                });

                var snapshots = await Task.WhenAll(tasks);

                // 🔹 Extraer los documentos
                var posts = snapshots
                    .SelectMany(snapshot => snapshot.Documents)
                    .Select(doc => doc.ConvertTo<Post>())
                    .ToList();

                return posts;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
        public async Task<List<Post>?> GetPostFromContestMostLiked(string contestID)
        {
            if (string.IsNullOrEmpty(contestID)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    .WhereEqualTo("ContestId", contestID)
                    .OrderByDescending("LikesCount")
                    .Limit(2);
                var Snapshot = await query.GetSnapshotAsync();
                if(Snapshot.Count > 0 )
                {
                    if(Snapshot.Count == 1)
                    {
                        var a = Snapshot[0].ConvertTo<Post>();
                        return new List<Post> { a };
                    }
                    else
                    {

                        var a =Snapshot[0].ConvertTo<Post>();
                        var b = Snapshot[1].ConvertTo<Post>();
                        if (a.LikesCount > b.LikesCount)
                        {
                            return new List<Post> { a };
                        }
                        if (a.LikesCount < b.LikesCount)
                        {
                            return new List<Post> { b };
                        }
                        if(a.LikesCount == b.LikesCount)
                        {
                            return new List<Post> { a, b };
                        }
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
         // cosas de admin 
        public async Task<List<Post>?> GetPendingPostPaginated(System.DateTime? dateTime)
        {
            List<Post> allPosts = new List<Post>();

            try
            {
                Query query = _firestoreDb.Collection(_collectionName)
                    .WhereEqualTo("Status", "pending")
                    .OrderByDescending("CreateAt")
                    .Limit(20);

                // Si se pasa una fecha, buscar los posts creados ANTES de esa fecha
                if (dateTime.HasValue)
                {
                    query = query.StartAfter(System.DateTime.UtcNow);
                }

                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        Post post = doc.ConvertTo<Post>();
                        allPosts.Add(post);
                    }
                }

                return allPosts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }

    
}
