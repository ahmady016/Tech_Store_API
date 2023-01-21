using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TechStoreApi.Common;

public interface IFileService
{
    string BaseAlbumDir { get; }
    string BaseContentDir { get; }

    void CreateDirectory(string rootDir, string dirName);
    Task<string> CreateFile(IFormFile formFile, string rootDir, string dirName, string fileName);
    Task<string> TryCreateThumb(IFormFile photoFile, string sourcePhotoName, string rootDir, string dirName, string photoName);
    void DeleteDirectory(string rootDir, string dirName);
    void DeleteFile(string rootDir, string dirName, string fileName);
}

public class FileService : IFileService
{
    private readonly int _thumbWidth = 400;
    public static string DefaultPhotoUrl => "https://my.ussu.co.uk/activity/PublishingImages/Logos/Ahlul%20Bayt.png";
    private readonly IWebHostEnvironment _env;
    public string BaseAlbumDir { get; }
    public string BaseContentDir { get; }
    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        BaseAlbumDir = Path.Combine(_env.WebRootPath, "Uploads", "Albums");
        BaseContentDir = Path.Combine(_env.WebRootPath, "Uploads", "Content");
    }

    private string _GetUniqueFileName(string Id, IFormFile formFile)
    {
        if (formFile is null || Id is null)
            return null;
        var fileExtension = Path.GetExtension(formFile.FileName);
        return $"{Id}{fileExtension}";
    }
    private async Task _CreateFile(IFormFile formFile, string filePath)
    {
        if (filePath is not null && formFile is not null)
        {
            await using FileStream fs = File.Create(filePath);
            await formFile.CopyToAsync(fs);
        }
    }
    private Size _GetNewImageSize(int currentWidth, int currentHeight, int newWidth)
    {
        var aspectRatio = (double) newWidth / (double) currentWidth;
        return new Size(
            Convert.ToInt32(currentWidth * aspectRatio),
            Convert.ToInt32(currentHeight * aspectRatio)
        );
    }
    private Image _ResizeImage(string imagePath, int newWidth)
    {
        if (imagePath is not null)
        {
            var image = Image.Load(imagePath);
            if (newWidth < image.Width)
            {
                var newSize = _GetNewImageSize(image.Width, image.Height, newWidth);
                if(newSize.Width > 0 && newSize.Height > 0)
                {
                    image.Mutate(action => action.Resize(newSize));
                    return image;
                }
            }
        }
        return null;
    }
    private async Task _CreateImage(Image image, string rootDir, string dirName, string fileName)
    {
        if (image is not null)
        {
            var imagePath = Path.Combine(rootDir, dirName, fileName);
            await image.SaveAsync(imagePath);
        }
    }

    public void CreateDirectory(string rootDir, string dirName)
    {
        Directory.CreateDirectory(Path.Combine(rootDir, dirName));
    }
    public void DeleteDirectory(string rootDir, string dirName)
    {
        var dirPath = Path.Combine(rootDir, dirName);
        if(Directory.Exists(dirPath))
        {
            var dirInfo = new DirectoryInfo(dirPath);
            // delete each sub directory with its files
            foreach (var dir in dirInfo.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                    file.Delete();
                dir.Delete(true);
            }
            // delete each file in root directory
            foreach (var file in dirInfo.GetFiles())
                file.Delete();
            // finally delete the directory
            dirInfo.Delete(true);
        }
    }
    public void DeleteFile(string rootDir, string dirName, string fileName)
    {
        var filePath = Path.Combine(rootDir, dirName, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
    public async Task<string> CreateFile(IFormFile formFile, string rootDir, string dirName, string fileName)
    {
        var uniqueFileName = _GetUniqueFileName(fileName, formFile);
        var filePath = Path.Combine(rootDir, dirName, uniqueFileName);
        await _CreateFile(formFile, filePath);
        return uniqueFileName;
    }
    public async Task<string> TryCreateThumb(IFormFile photoFile, string sourcePhotoName, string rootDir, string dirName, string photoName)
    {
        var photoPath = Path.Combine(rootDir, dirName, sourcePhotoName);
        var image = _ResizeImage(photoPath, _thumbWidth);
        if(image is not null)
        {
            var uniquePhotoName = _GetUniqueFileName($"{photoName}_thumb", photoFile);
            await _CreateImage(image, rootDir, dirName, uniquePhotoName);
            return uniquePhotoName;
        }
        return null;
    }
}
