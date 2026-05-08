using Aplication.Interfaces.Repository;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Domain.Entities.Notification;


namespace Infrastructure.Services.Jobs
{

    public class ConquestFinaliceJob : IJob
    {
        private readonly IContestRespository _contestRespository;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IPostRepository _postRepository;
        private readonly IProfileRepository _profileRepository;

        public ConquestFinaliceJob(IProfileRepository profileRepository, IPostRepository postRepository, IContestRespository contestRespository, IFirebaseMessagingRepository firebaseMessagingRepository)
        {
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _contestRespository = contestRespository;
            _postRepository = postRepository;
            _profileRepository = profileRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("finaliza Concurzo");
            try
            {
                string? ContestID = context.JobDetail.JobDataMap.GetString("ContestID");
                var contest = await _contestRespository.GetContestById(ContestID);
                var post = await _postRepository.GetPostFromContestMostLiked(contest.Id);
                if(post != null)
                {
                    if (post.Count == 2)
                    {
                        _contestRespository.AddDaysToContest(contest.Id, 3);
                        var notificaion = new NotificationEntity()
                        {
                            Title = "Se empato en el concurso" + contest.Title,
                            Body = "el concurso se extendera 3 dias",
                            ProfileId = "gloval",
                            Type = "CotestWiner",
                            Data = new Dictionary<string, object?>
                            {
                                {"url",post[0].Url }
                            }

                        };
                        await _firebaseMessagingRepository.SendAndSaveNotification(notificaion, new List<string>());
                    }
                    else if (post.Count == 1)
                    {
                        var notificaion = new NotificationEntity()
                        {
                            Title = "Algien a ganado el concurso " + contest.Title + " !!!!",
                            Body = "Se a desidio un Post ganador del concurso, entra para verllo!",
                            ProfileId = "gloval",
                            Type = "CotestWiner",
                            Data = new Dictionary<string, object?>
                            {
                                {"url",post[0].Url }
                            }

                        };
                        var profile = await _profileRepository.GetProfileByIdAsync(post[0].IdPublisher);
                        var notificaion1 = new NotificationEntity()
                        {
                            Title = "As gando el concurso " + contest.Title + " !!!!",
                            Body = "Tu post tuvo mas likes que nunguno !",
                            ProfileId = profile.Id,
                            Type = "CotestWiner",
                            Data = new Dictionary<string, object?>
                            {
                                {"url",post[0].Url }
                            }

                        };
                        var list = profile.User1DeviceTokens.Concat(profile.User2DeviceTokens);
                        var listToNotification = new List<string>();
                        foreach (var item in list)
                        {
                            listToNotification.Add(item.Token);
                        }

                        await _firebaseMessagingRepository.SendAndSaveNotification(notificaion, new List<string>());
                        await _firebaseMessagingRepository.SendAndSaveNotification(notificaion1, listToNotification);

                        //await _firebaseMessagingRepository.SendGlobalNotificationByTopicAsync("global");

                }
                    await _contestRespository.FinishedContest(ContestID);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            



        }
    }
}
