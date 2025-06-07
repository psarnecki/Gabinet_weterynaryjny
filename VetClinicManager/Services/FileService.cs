namespace VetClinicManager.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> SaveFileAsync(IFormFile file, string subfolder)
    {
        if (file == null || file.Length == 0) return null;

        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, subfolder);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/{subfolder.Replace('\\', '/')}/{uniqueFileName}";
    }

    public void DeleteFile(string? fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;
        
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}