using System.IO;
using System.Threading.Tasks;

namespace OrderItemsReserver.Services.Storage.Interfaces
{
    public interface IBlobStorageService
    {
        Task DownloadBlobIfExistsAsync(Stream stream, string blobName);
        Task<string> UploadBlobAsync(Stream stream, string name);
        Task DeleteBlobIfExistsAsync(string blobName);
        Task<bool> DoesBlobExistAsync(string blobName);
        Task<string> GetBlobUrl(string blobName);
        string GenerateSasTokenForContainer();
    }
}
