using System.Diagnostics;
using CVDMBlog.Data.Repository;
using CVDMBlog.Data.Repository.FileManager;
using CVDMBlog.Models;
using CVDMBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVDMBlog.Controllers;

[Authorize(Roles = "Admin")]
public class PanelController : Controller
{
    private IRepository _repo;
    private IFileManager _fileManager;
    
    public PanelController(
        IRepository repo,
        IFileManager fileManager
        )
    {
        _repo = repo;
        _fileManager = fileManager;
    }
    public IActionResult Index()
    {
        var posts = _repo.GetAllPosts();
        return View(posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Edit(string? slug)
    {
        if (slug  == null)
        {
            return View(new PostViewModel());
        }
        else
        {
            var post = _repo.GetPost((string) slug);
            return View(new PostViewModel
            {
                Id = post.Id, 
                Title = post.Title,
                Body = post.Body,
                
            });
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(PostViewModel vm)
    {
        var post = vm.Id > 0 ? _repo.GetPostById(vm.Id) : new Post();

        post.Title = vm.Title;
        post.Body = vm.Body;

        if (vm.Image != null)
        {
            var imageResult = await _fileManager.SaveImage(vm.Image);
            if (imageResult != "Error")
            {
                post.Image = imageResult;
            }
        }

        // Generate or update the slug based on the title
        post.UpdateSlug();

        if (post.Id > 0)
        {
            _repo.UpdatePost(post);
        }
        else
        {
            _repo.AddPost(post);
        }

        if (await _repo.SaveChangesAsync())
        {
            return RedirectToAction("Index");
        }
        else
        {
            return View(vm);
        }
    }

    
    [HttpGet]
    public async Task<IActionResult> Remove(string slug)
    {
        _repo.RemovePost(slug);
        await _repo.SaveChangesAsync();
        return RedirectToAction("Index");

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}