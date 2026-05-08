using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IPostRepository
    {
        Task<Post?> GetPostById(string id);
        Task<List<Post>?> GetSelfProfilePosts(string id);
        Task<List<Post>?> GetFriendProfilePosts(string id);
        Task<List<Post>?> GetAceptedPublicProfilePosts(string id);
        Task<bool?> UpdatePostById(string id, Post post);
        Task<string?> UploadPost(Post post);
        Task<bool?> DeletePostById(string id);
        Task<bool?> DeletePostWithIpPublisher(string postId, string publisherId);
        Task<List<Post>?> GetLastPublicPostPaginated(DateTime? dateTime);
        Task<List<Post>?> GetLastedFriendsPostPaginated(DateTime? dateTime, List<string> ids);
        Task<List<string>?> GedtALFacePostFromProfileId(string profileId);
        Task<List<Post>?> GetPostByList(List<string> ids);

        Task<List<Post>?> GetPostFromContestMostLiked(string contestID);
        Task<List<Post>?> GetPendingPostPaginated(System.DateTime? dateTime);
    }
}
