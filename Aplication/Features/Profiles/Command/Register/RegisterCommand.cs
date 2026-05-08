using Aplication.DTO.InputDto.Register;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.Register
{
    public class RegisterCommand: IRequest<Response<bool>>
    {
        public int cantidadUsuraios {  get; set; }
        public RegisterUserDto? registerUserDtos1 { get; set; }
        public RegisterUserDto? registerUserDtos2 { get; set; }
        public RegisterProfileDto? profileDtos { get; set; }
        [FirestoreProperty]
        public IFormFile? FacePhotoUser1 { get; set; }
    
        [FirestoreProperty]
        public IFormFile? FacePhotoUser2 { get; set; }
        
    }
}

