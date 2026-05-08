using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Google.Api.Gax.Grpc;
using MediatR;

namespace Aplication.Features.test.Command
{
    public class SendNotificationCommandHandler: IRequestHandler<SendNotificationCommand, string>
    {
        private readonly IFirebaseMessagingRepository _firebaseMessagingService;
        public SendNotificationCommandHandler(IFirebaseMessagingRepository firebaseMessagingService) 
        {
            _firebaseMessagingService = firebaseMessagingService;
        }

        public async Task<string> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            return await _firebaseMessagingService.SendNotificationAsync(request.notificationRequest.DeviceToken);
        }
    }
}
