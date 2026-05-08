using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.CreateChat
{
    internal class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Response<ChatDocument?>>
    {
        private readonly IAuthService _authService;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IChatRepository _chatRepository;

        public CreateChatCommandHandler(IAuthService authService, IFriendshipRepository friendshipRepository, IJwtTokenService jwtTokenService, IChatRepository chatRepository)
        {
            _authService = authService;
            _friendshipRepository = friendshipRepository;
            _jwtTokenService = jwtTokenService;
            _chatRepository = chatRepository;
        }

        public async Task<Response<ChatDocument?>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {

            if (_authService.HasNullPropertiesLinq(request)) return new Response<ChatDocument?> { succeeded = true, data = null };
            if (request.Principal == null || string.IsNullOrEmpty(request.Profile2Id)) return new Response<ChatDocument?> { succeeded = true, data = null };
            var selfProfileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            try
            {
                var friends = await _friendshipRepository.ExistFrienship(selfProfileId!, request.Profile2Id);
                if (friends == null || friends == false) return new Response<ChatDocument?> { succeeded = true, data = null };
                var a = await _chatRepository.getChatbByTwoProfil(selfProfileId!, request.Profile2Id);
                if (a != null)
                {
                    return new Response<ChatDocument?> { succeeded = true, data = a };
                }
                a = await _chatRepository.CreateChatAsync(selfProfileId!, request.Profile2Id,request.SelfProfileName!,request.FriendProfileName!) ;
                return new Response<ChatDocument?> { succeeded = true, data = a };
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
                return new Response<ChatDocument?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
