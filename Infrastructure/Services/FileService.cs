using Aplication.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class FileService: IFileService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "proyectox-f9e00.firebasestorage.app";

        public FileService()
        {
            GoogleCredential credential;
            var credentialsJson = $@"
            {{
                ""type"": ""{Environment.GetEnvironmentVariable("type")}"",
                ""project_id"": ""{Environment.GetEnvironmentVariable("project_id")}"",
                ""private_key_id"": ""{Environment.GetEnvironmentVariable("private_key_id")}"",
                ""private_key"": ""{Environment.GetEnvironmentVariable("private_key")}"",
                ""client_email"": ""{Environment.GetEnvironmentVariable("client_email")}"",
                ""client_id"": ""{Environment.GetEnvironmentVariable("FIREBASE_CLIENT_ID")}"",
                ""auth_uri"": ""{Environment.GetEnvironmentVariable("auth_uri")}"",
                ""token_uri"": ""{Environment.GetEnvironmentVariable("token_uri")}"",
                ""auth_provider_x509_cert_url"": ""{Environment.GetEnvironmentVariable("auth_provider_x509_cert_url")}"",
                ""client_x509_cert_url"": ""{Environment.GetEnvironmentVariable("client_x509_cert_url")}"",
                ""universe_domain"": ""{Environment.GetEnvironmentVariable("universe_domain")}""
            }}";
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("type")))
            {

                // Cargar las credenciales desde el JSON generado a partir de las variables de entorno
                credential = GoogleCredential.FromJson(credentialsJson);
            }
            else
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-credentials.json");
                Console.WriteLine(path);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                credential = GoogleCredential.FromFile(path);
            }
            
            _storageClient = StorageClient.Create(credential);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // subimos al bucket
            var obj = await _storageClient.UploadObjectAsync(
                _bucketName,
                fileName,
                contentType,
                fileStream
            );

            // la URL pública
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media";
        }
    }
}

