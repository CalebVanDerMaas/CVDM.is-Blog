using System.Runtime.InteropServices.JavaScript;

namespace CVDMBlog.Data.Repository.FileManager;

public class FileManager : IFileManager
{
    private string _imagePath;
    public FileManager(IConfiguration config)
    {
        _imagePath = config["Path:Images"];
    }

    public FileStream ImageStream(string image)
    {
        return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
    }

    public async Task<String> SaveImage(IFormFile image)
    {
        try
        {
            var save_path = Path.Combine(_imagePath);
            if (!Directory.Exists(save_path))
            {
                Directory.CreateDirectory(save_path);
            }

            //Internet Explorer Error
            //var filename = image.FileName;
            var mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
            var fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";

            using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return fileName;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        
    }
}