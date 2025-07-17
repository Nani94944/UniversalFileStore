using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using UniversalFileStore.Models;

namespace UniversalFileStore.Services;

public class GoogleDriveService : IFileStorageService
{
    private readonly DriveService _driveService;
    private readonly string _applicationName;

    public GoogleDriveService ( IConfiguration configuration )
    {
        _applicationName=configuration["GoogleDrive:ApplicationName"];
        string credentialsPath = configuration["GoogleDrive:CredentialsPath"];
        using var stream = new FileStream ( credentialsPath , FileMode.Open , FileAccess.Read );
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync (
            GoogleClientSecrets.FromStream ( stream ).Secrets ,
            new[] { DriveService.Scope.DriveFile } ,
            "user" ,
            CancellationToken.None ,
            new FileDataStore ( "token" , true ) ).Result;

        _driveService=new DriveService ( new BaseClientService.Initializer
        {
            HttpClientInitializer=credential ,
            ApplicationName=_applicationName
        } );
    }

    public async Task<FileResponse> UploadFileAsync ( IFormFile file , string fileName )
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name=fileName
        };

        using var memoryStream = new MemoryStream ();
        await file.CopyToAsync ( memoryStream );
        memoryStream.Position=0;

        var request = _driveService.Files.Create ( fileMetadata , memoryStream , file.ContentType );
        request.Fields="id, webViewLink";
        var uploadedFile = await request.UploadAsync ();
        var fileId = (await _driveService.Files.Get ( request.ResponseBody.Id ).ExecuteAsync ()).Id;

        string checksum = ComputeChecksum ( memoryStream );
        string downloadUrl = $"https://drive.google.com/uc?export=download&id={fileId}";

        return new FileResponse
        {
            Provider="gdrive" ,
            FileId=fileId ,
            Checksum=checksum ,
            DownloadUrl=downloadUrl
        };
    }

    public async Task<Stream> DownloadFileAsync ( string fileId )
    {
        var request = _driveService.Files.Get ( fileId );
        var stream = new MemoryStream ();
        await request.DownloadAsync ( stream );
        stream.Position=0;
        return stream;
    }

    private string ComputeChecksum ( Stream stream )
    {
        stream.Position=0;
        using var sha256 = SHA256.Create ();
        byte[] hash = sha256.ComputeHash ( stream );
        return BitConverter.ToString ( hash ).Replace ( "-" , "" ).ToLower ();
    }
}