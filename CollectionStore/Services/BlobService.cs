using Azure.Storage.Blobs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class BlobService : IBlobService
    {
        private const string ContainerName = "collection-covers";
        private readonly BlobServiceClient blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        public async Task<BlobInfo> GetBlobAsync(string blobName)
        {
            try
            {
                blobName ??= string.Empty;
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                var blobDownloadInfo = await blobClient.DownloadAsync();
                return new BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> UploadFileBlobAsync(string filePath, string fileName)
        {
            try
            {
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = await GetFreeBlobClient(fileName, containerClient);
                await blobClient.UploadAsync(filePath);
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public async Task<bool> UploadFileBlobAsync(Stream stream, string fileName)
        {
            try
            {
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = await GetFreeBlobClient(fileName, containerClient);
                await blobClient.UploadAsync(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            try
            {
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<BlobClient> GetFreeBlobClient(string blobName, BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            int count = 1;
            while ((await blobClient.ExistsAsync()).Value)
            {
                blobClient = containerClient.GetBlobClient(GetNewName(blobName, count++));
            }
            return blobClient;
        }
        private string GetNewName(string fileName, int count)
        {
            int index = fileName.LastIndexOf('.');
            return fileName.Insert(index, $"_{count}");
        }
    }
}
