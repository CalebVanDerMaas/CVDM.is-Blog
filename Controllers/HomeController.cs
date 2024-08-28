using System.Diagnostics;
using CVDMBlog.Data;
using CVDMBlog.Data.Repository;
using CVDMBlog.Data.Repository.FileManager;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CVDMBlog.Models;

namespace CVDMBlog.Controllers;

public class HomeController : Controller
{
    private IRepository _repo;
    private IFileManager _fileManager;
    
    public HomeController(
        IRepository repo,
        IFileManager fileManager
        )
    {
        _repo = repo;
        _fileManager = fileManager;
    }
    // private readonly ILogger<HomeController> _logger;
    //
    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    public IActionResult Index()
    {
        var posts = _repo.GetAllPosts();
        return View(posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public IActionResult Post(int id)
    {
        var post = _repo.GetPost(id);
        return View(post);
    }

    [HttpGet("/Image/{image}")]
    public IActionResult Image(string image)
    {
        var mime = image.Substring(image.LastIndexOf('.') + 1);
        return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
    }
}