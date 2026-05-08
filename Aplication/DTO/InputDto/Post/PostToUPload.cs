using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Post
{
    public class PostToUpload
    {
        public string? ProfileName {  get; set; }
        public string? PostType { get; set; }
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
        public string? ContestId { get; set; }
    }
}
