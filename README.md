Universal File-Store Connector
A .NET Core Web API to upload and download files to Google Drive and Dropbox using a unified JSON interface.
Prerequisites

.NET 8 SDK
Visual Studio (for IIS Express)
Google Drive API credentials.json
Dropbox API access token (optional)
curl or Postman for testing

Setup

Clone the Repository (if applicable):git clone <repository-url>
cd UniversalFileStore


Restore Dependencies:dotnet restore


Add Google Drive Credentials:
Create credentials.json in the project root:{"web":{"client_id":"759973062857-ffiouenipdfct0gmi3ih64m48j0a66c0.apps.googleusercontent.com","project_id":"universalfilestore","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"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","redirect_uris":["https://localhost:44323/oauth2callback"]}}




Configure appsettings.json:
Ensure it contains:{
  "GoogleDrive": {
    "CredentialsPath": "credentials.json",
    "ApplicationName": "UniversalFileStore"
  },
  "Dropbox": {
    "AccessToken": "<your-dropbox-access-token>"
  }
}




Run with IIS Express:
In Visual Studio, select IIS Express and press F5.
Or use:dotnet run --launch-profile "IIS Express"


The API runs on https://localhost:44323.



Google Drive OAuth Setup

First Run:
A browser will open for OAuth authentication.
Sign in with a test user account and allow the drive.file scope.
Tokens are stored in bin/Debug/net8.0/token.


Google Cloud Console:
Ensure the OAuth 2.0 Client ID has:
Authorized redirect URIs: https://localhost:44323/oauth2callback
Authorized JavaScript origins: Empty





API Endpoints

Upload File:curl -k -F file=@<path-to-file> https://localhost:44323/upload?provider=<gdrive|dropbox>

Example Response:{
  "provider": "gdrive",
  "fileId": "<file-id>",
  "checksum": "<sha256-checksum>",
  "downloadUrl": "https://drive.google.com/uc?export=download&id=<file-id>"
}


Download File:curl -k https://localhost:44323/download?id=<file-id>&provider=<gdrive|dropbox> -o <output-file>



Testing with Postman

Upload:
Method: POST
URL: https://localhost:44323/upload?provider=<gdrive|dropbox>
Body: Form-data, key=file, value=select a file (e.g., logo.png)
Disable SSL verification in Postman settings.


Download:
Method: GET
URL: https://localhost:44323/download?id=<file-id>&provider=<gdrive|dropbox>
Save response as a file.



Notes

Ensure credentials.json is set to "Copy to Output Directory: Always" in .csproj.
For Dropbox, provide a valid access token in appsettings.json.
Use /upload and /download endpoints, not /filestore/*.
