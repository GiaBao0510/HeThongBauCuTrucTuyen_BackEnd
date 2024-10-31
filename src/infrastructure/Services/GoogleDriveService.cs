using BackEnd.src.core.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System.Text.Json;

namespace BackEnd.src.infrastructure.Services
{
    // Tạo CustomTokenStore để lưu token theo định dạng mong muốn
    public class CustomTokenStore : IDataStore
    {
        private readonly string _tokenFilePath;

        public CustomTokenStore(string tokenFilePath)
        {
            _tokenFilePath = tokenFilePath;
        }

        Task IDataStore.ClearAsync()
        {
            if (File.Exists(_tokenFilePath))
            {
                File.Delete(_tokenFilePath);
            }
            return Task.CompletedTask;
        }

        Task IDataStore.DeleteAsync<T>(string key)
        {
            if (File.Exists(_tokenFilePath))
            {
                File.Delete(_tokenFilePath);
            }
            return Task.CompletedTask;
        }

        async Task<T> IDataStore.GetAsync<T>(string key)
        {
            if (!File.Exists(_tokenFilePath))
            {
                return default;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_tokenFilePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        async Task IDataStore.StoreAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_tokenFilePath, json);
        }
    }

    public class GoogleDriveService
    {
        private static readonly string[] Scopes = { DriveService.Scope.DriveFile };
        private readonly DriveService _driveService;
        private readonly string _credentialsPath;
        private readonly string _tokenPath;
        private readonly string _tokenFilePath;

        public GoogleDriveService(string credentialsPath, string tokenPath)
        {
            _credentialsPath = credentialsPath;
            _tokenPath = tokenPath;
            _tokenFilePath = Path.Combine(_tokenPath, "token.json");
            _driveService = InitializeDriveService().Result;
        }

        private async Task<DriveService> InitializeDriveService()
        {
            UserCredential credential;
            
            try
            {
                // Tạo thư mục token nếu chưa tồn tại
                if (!Directory.Exists(_tokenPath))
                {
                    Directory.CreateDirectory(_tokenPath);
                }

                // Tạo CustomTokenStore với đường dẫn token.json
                var tokenStore = new CustomTokenStore(_tokenFilePath);

                using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        tokenStore);  // Sử dụng CustomTokenStore
                }

                // Đảm bảo token file có quyền ghi
                if (File.Exists(_tokenFilePath))
                {
                    var fileInfo = new FileInfo(_tokenFilePath);
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        fileInfo.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize Google Drive service: {ex.Message}", ex);
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "VoteSecure"
            });
        }

        //Thêm tệp tin dựa trên ID thư mục
        public async Task<bool> UploadFileAsync(string filePath, string filename, string mimeType)
        {
            var fileMetaData = new Google.Apis.Drive.v3.Data.File
            {
                Name = filename,
                MimeType = mimeType,
                Parents = new[] { "1lfcfq-fMMOMXQlXwSLYUGkpJDVDoMM7U" } //ID thư mục để lưu file
            };

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var request = _driveService.Files.Create(fileMetaData, stream, mimeType);
            request.Fields = "id";

            var response = await request.UploadAsync();
            if (response.Status != UploadStatus.Completed){
                Console.WriteLine($"Error uploading file: {response.Exception.Message}");
                return false;
            }
            Console.WriteLine($"File uploaded successfully: {filename} - request.ResponseBody.Id:{request.ResponseBody.Id}");
            return true;
        }

        //Xóa tệp tin dựa trên ID thư mục
        public async Task<bool> deleteFileAssync(string fileName, string folderId){
            try{
                //tim file dựa trên ID thư mục chứa nó
                var request = _driveService.Files.List();
                request.Q = $"name='{fileName}' and '{folderId}' in parents and trashed=false";
                request.Fields = "files(id, name)";

                var result = await request.ExecuteAsync();
                var file = result.Files.FirstOrDefault();

                if(file != null){
                    //Xóa tệp tin 
                    await _driveService.Files.Delete(file.Id).ExecuteAsync();
                    Console.WriteLine($"File deleted successfully: {fileName}");
                    return true;
                }
                else{
                    Console.WriteLine($"File not found: {fileName}");
                    return false;
                }
            }            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa tệp tin: {ex.Message}");
                return false;
            }
        }
    
    }
}