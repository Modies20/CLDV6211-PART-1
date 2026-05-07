using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace EventEase.Services
{
    public class AzureBlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobService> _logger;
        public AzureBlobService(IConfiguration configuration, ILogger<AzureBlobService> logger)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }
        public async Task<string> UploadImageAsync(IFormFile file, string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
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
                _logger.LogError(ex, "Failed to upload image");
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
