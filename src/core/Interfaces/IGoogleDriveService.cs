
namespace BackEnd.src.core.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<String> UploadFileAsync(string filePath, string filename, string mimeType);
    }
}