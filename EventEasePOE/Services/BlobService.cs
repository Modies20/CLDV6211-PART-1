using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase.Services
{
    public interface IBlobService
    {
        Task<string> UploadImageAsync(IFormFile file, string containerName);
        Task DeleteImageAsync(string blobUrl, string containerName);
    }

    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobService> _logger;

        public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
        {
            var connectionString = configuration.GetConnectionString("BlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Generate a unique filename to avoid collisions
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                _logger.LogInformation($"Uploaded {fileName} to {containerName}");
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Image upload failed");
                throw;
            }
        }

        public async Task DeleteImageAsync(string blobUrl, string containerName)
        {
            if (string.IsNullOrEmpty(blobUrl)) return;

            try
            {
                var uri = new Uri(blobUrl);
                var fileName = Path.GetFileName(uri.LocalPath);
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete blob {blobUrl}");
            }
        }
    }
}