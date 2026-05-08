using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ImageService: IImageService
    {
        private readonly HttpClient _httpClient;

        public ImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(Stream?, string?)> GetImageStreamAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL is required.", nameof(url));

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error fetching image: {response.StatusCode}");

            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";

            return (stream, contentType);
        }
    }
}
