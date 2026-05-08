using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Command.LikeDislike
{
    internal class LikeDislikeCoommandHandler : IRequestHandler<LikeDislikeCoommand, Response<bool?>>
    {
       
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IPostRepository _postRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LikeDislikeCoommandHandler(IFirebaseMessagingRepository firebaseMessagingRepository, IProfileRepository profileRepository, IPostRepository postRepository, IJwtTokenService jwtTokenService)
        {
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _profileRepository = profileRepository;
            _postRepository = postRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(LikeDislikeCoommand request, CancellationToken cancellationToken)
        {
            var profileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            try
            { 
                var post = await _postRepository.GetPostById(request.PostId);
                if (post != null)
                {
                    if (post.Likes == null) post.Likes = new List<string>();
                    if (request.State == true)
                    {
                        if (!post.Likes.Contains(profileId))
                        {
                            post.Likes.Add(profileId);
                        }
                        if (post.LikesCount != null)
                        {
                            post.LikesCount = post.LikesCount + 1;
                        }
                        else
                        {
                            post.LikesCount = 1;
                        }
                        Console.WriteLine("El id es: " + post.IdPublisher ?? "NULO");
                        
                       
                        var profilePublisher = await _profileRepository.GetProfileByIdAsync(post.IdPublisher!); //esta es la linea 50

                        var profileLiked = await _profileRepository.GetProfileByIdAsync(profileId);
                        var notification = new NotificationEntity
                        {
                            Title = "A alguien le gusta tu Post",
                            Body = "A " + profileLiked.NameProfile + " le a gustado tu foto.",
                            Type = "Info",
                            ProfileId= post.IdPublisher!,
                            Data = new Dictionary<string, object?> {
                            { "SenderName",profileLiked.NameProfile }


                        }

                        };
                        var listResult = profilePublisher.User1DeviceTokens.Concat(profilePublisher.User2DeviceTokens).ToList();
                        List<string> list = new List<string>();
                        foreach (var item in listResult)
                        {
                            list.Add(item.Token);
                        }
                        await _firebaseMessagingRepository.SendAndSaveNotification(notification, list);

                        

                    }
                    if (request.State == false)
                    {
                        if (post.Likes.Contains(profileId))
                        {
                            post.Likes.Remove(profileId);
                        }
                        if (post.LikesCount != null)
                        {
                            post.LikesCount = Math.Max(0, post.LikesCount.Value - 1);
                        }
                    }
                   



                    var result = await _postRepository.UpdatePostById(request.PostId, post);
                    return new Response<bool?> { succeeded = true, data = result };
                }
                return new Response<bool?> { succeeded = true, data = false };
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

                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
