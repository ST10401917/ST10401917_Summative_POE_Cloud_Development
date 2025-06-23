using Azure.Storage.Blobs;
namespace EventEase.Services
{
    public interface IBlobService
    {
        Task<string> UploadAsync(Stream file, string fileName, string contentType);
        Task DeleteAsync(string blobName);

    }

    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IConfiguration cfg)
        {
            var conn = cfg["Storage:ConnectionString"];
            var containName = cfg["Storage:Container"];
            _container = new BlobContainerClient(conn, containName);
            _container.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }

        public async Task<string> UploadAsync(Stream file, string fileName, string contentType)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(file, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType });
            return blob.Uri.ToString();
        }

        public async Task DeleteAsync(string blobName)
        {
            var blob = _container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync();
        }

    }
}
