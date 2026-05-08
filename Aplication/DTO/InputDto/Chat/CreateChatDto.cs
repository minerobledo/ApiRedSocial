using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Chat
{
    public class CreateChatDto
    {
        public string? SelfProfileName { get; set; }
        public string? FriendId { get; set; }
        public string? FriendProfileName { get; set; }

        //public string? SelfPublicKey { get; set; }
        //public string? FriendPublicKey { get; set; }
    }
}
