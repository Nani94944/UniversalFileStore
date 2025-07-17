using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using UniversalFileStore.Models;

namespace UniversalFileStore.Services;

public class DropboxService : IFileStorageService
{
    private readonly DropboxClient _dropboxClient;

    public DropboxService ( IConfiguration configuration )
    {
        var accessToken = configuration["Dropbox:AccessToken"];
        _dropboxClient=new DropboxClient ( accessToken );
    }

    public async Task<FileResponse> UploadFileAsync ( IFormFile file , string fileName )
    {
        using var memoryStream = new MemoryStream ();
        await file.CopyToAsync ( memoryStream );
        memoryStream.Position=0;

        var filePath = $"/{fileName}";
        var uploadedFile = await _dropboxClient.Files.UploadAsync (
            path: filePath ,
            mode: WriteMode.Overwrite.Instance ,
            body: memoryStream );

        string checksum = ComputeChecksum ( memoryStream );
        var sharedLink = await _dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync ( uploadedFile.PathLower );

        return new FileResponse
        {
            Provider="dropbox" ,
            FileId=uploadedFile.Id ,
            Checksum=checksum ,
            DownloadUrl=sharedLink.Url.Replace ( "?dl=0" , "?dl=1" )
        };
    }

    public async Task<Stream> DownloadFileAsync ( string fileId )
    {
        var fileMetadata = await _dropboxClient.Files.GetMetadataAsync ( fileId );
        var downloadArg = new DownloadArg ( fileMetadata.AsFile.PathLower );
        using var response = await _dropboxClient.Files.DownloadAsync ( downloadArg );
        return await response.GetContentAsStreamAsync ();
    }

    private string ComputeChecksum ( Stream stream )
    {
        stream.Position=0;
        using var sha256 = SHA256.Create ();
        byte[] hash = sha256.ComputeHash ( stream );
        return BitConverter.ToString ( hash ).Replace ( "-" , "" ).ToLower ();
    }
}