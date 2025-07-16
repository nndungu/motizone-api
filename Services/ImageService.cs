namespace MotiZone.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env)
    {
        _env = env;
    }
    public List<string> GetImagesFromFolder(string relativeFolderPath)
    {
        var wwwRootPath = _env.WebRootPath;
        var fullPath = Path.Combine(wwwRootPath, relativeFolderPath);

        if (!Directory.Exists(fullPath))
            return new List<string>();

        var files = Directory.GetFiles(fullPath)
            .Select(Path.GetFileName)
            .Select(fileName => $"/{relativeFolderPath}/{fileName}")
            .ToList();

        return files;
    }
    public async Task<string> SaveImageAsync(IFormFile imageFile, string folder)
    {
        if (imageFile == null || imageFile.Length == 0) return null;

        var uploadsPath = Path.Combine(_env.WebRootPath, "images", folder);
        Directory.CreateDirectory(uploadsPath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var fullPath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"/images/{folder}/{fileName}";
    }
}