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
}