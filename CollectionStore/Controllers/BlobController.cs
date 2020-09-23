using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService blobService;

        public BlobController(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        public async Task<IActionResult> GetBlob(string blobName)
        {
            var data = await blobService.GetBlobAsync(blobName);
            if(data != null)
            {
                return File(data.Content, data.ContentType);
            }
            return BadRequest();
        }
        public async Task<IActionResult> UploadFile(string filePath, string fileName)
        {
            bool result = await blobService.UploadFileBlobAsync(filePath, fileName);
            return result ? (Ok() as IActionResult) : BadRequest();
        }
        public async Task<IActionResult> DeleteFile(string blobName)
        {
            bool result = await blobService.DeleteBlobAsync(blobName);
            return result ? (Ok() as IActionResult) : BadRequest();
        }
    }
}
