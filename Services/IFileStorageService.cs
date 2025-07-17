using UniversalFileStore.Models;

namespace UniversalFileStore.Services;

public interface IFileStorageService
{
    Task<FileResponse> UploadFileAsync ( IFormFile file , string fileName );
    Task<Stream> DownloadFileAsync ( string fileId );
}