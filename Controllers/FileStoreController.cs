using Microsoft.AspNetCore.Mvc;
using UniversalFileStore.Models;
using UniversalFileStore.Services;

namespace UniversalFileStore.Controllers;

[ApiController]
[Route ( "" )]
public class FileStoreController : ControllerBase
{
    private readonly FileStorageFactory _fileStorageFactory;

    public FileStoreController ( FileStorageFactory fileStorageFactory )
    {
        _fileStorageFactory=fileStorageFactory;
    }

    [HttpPost ( "upload" )]
    public async Task<IActionResult> Upload ( [FromQuery] string provider , IFormFile file )
    {
        if (file==null||file.Length==0)
            return BadRequest ( "No file uploaded." );

        try
        {
            var storageService = _fileStorageFactory.GetStorageService ( provider );
            var response = await storageService.UploadFileAsync ( file , file.FileName );
            return Ok ( response );
        }
        catch (ArgumentException ex)
        {
            return BadRequest ( ex.Message );
        }
        catch (Exception ex)
        {
            return StatusCode ( 500 , $"Error uploading file: {ex.Message}" );
        }
    }

    [HttpGet ( "download" )]
    public async Task<IActionResult> Download ( [FromQuery] string id , [FromQuery] string provider )
    {
        try
        {
            var storageService = _fileStorageFactory.GetStorageService ( provider );
            var stream = await storageService.DownloadFileAsync ( id );
            return File ( stream , "application/octet-stream" );
        }
        catch (ArgumentException ex)
        {
            return BadRequest ( ex.Message );
        }
        catch (Exception ex)
        {
            return StatusCode ( 500 , $"Error downloading file: {ex.Message}" );
        }
    }
}