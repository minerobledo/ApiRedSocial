using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Aplication.Features.Posts.Command.UploadPost
{
    public class UploadPostCommand : IRequest<Response<Post?>>
    {
        public ClaimsPrincipal? Principal { get; set; }
        public string? ProfileName { get; set; }
        public string? PostType { get; set; }
        public string? Description { get; set; }
        public Stream PhotoStream { get; set; }
        public string? FileName { get; set; }
        public string? ContentType{ get; set; }
        public string? ContestId { get; set; }
    }
}
