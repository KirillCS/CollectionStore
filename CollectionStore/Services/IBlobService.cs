using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class BlobInfo
    {
        public Stream Content { get; set; }
        public string ContentType { get; set; }

        public BlobInfo(Stream content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }
    }

    public interface IBlobService
    {
        Task<BlobInfo> GetBlobAsync(string name);
        Task<bool> UploadFileBlobAsync(string filePath, string fileName);
        Task<bool> UploadFileBlobAsync(Stream stream, string fileName);
        Task<bool> DeleteBlobAsync(string blobName);
    }
}
