namespace VetClinicManager.Services;

public interface IFileService
{
    Task<string?> SaveFileAsync(IFormFile file, string subfolder);
    void DeleteFile(string? fileUrl);
}