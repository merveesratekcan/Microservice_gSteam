using GameService.DTOs;

namespace GameService.Services
{
    public interface IFileService
    {
       Task UploadImage();
       Task<string> UploadVideo(IFormFile File);
    }
}