using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.File
{
    public class UploadCommand: IRequest<Response<string>>
    {
        public string FileName {  get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }
    internal class UploadCommandHnlder : IRequestHandler<UploadCommand, Response<string>>
    {
        private readonly IFileService _fileService;

        public UploadCommandHnlder(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Response<string>> Handle(UploadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string fileName1 = $"Comprobantes/{Guid.NewGuid()}_{Path.GetFileName(request.FileName)}";

                // subir archivo al storage
                var url1 = await _fileService.UploadFileAsync(request.Stream, request.FileName, request.ContentType);
                return new Response<string>() { data= url1,succeeded = true};

            }catch (Exception ex)
            {
                return new Response<string>() { errors = new List<Exception> { ex } };

            }
        }
    }
}
