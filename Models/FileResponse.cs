namespace UniversalFileStore.Models;

public class FileResponse
{
    public string Provider { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public string Checksum { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
}