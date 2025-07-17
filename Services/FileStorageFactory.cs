using Microsoft.Extensions.Configuration;

namespace UniversalFileStore.Services;

public class FileStorageFactory
{
    private readonly IConfiguration _configuration;

    public FileStorageFactory ( IConfiguration configuration )
    {
        _configuration=configuration;
    }

    public IFileStorageService GetStorageService ( string provider )
    {
        return provider.ToLower () switch
        {
            "gdrive" => new GoogleDriveService ( _configuration ),
            "dropbox" => new DropboxService ( _configuration ),
            _ => throw new ArgumentException ( "Invalid provider specified." )
        };
    }
}