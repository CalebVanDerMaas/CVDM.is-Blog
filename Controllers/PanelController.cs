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
        var currentStatus = _repo.GetCurrentStatus();
        ViewBag.CurrentStatus = currentStatus;
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
    
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string content)
    {
        var status = _repo.GetAllStatuses().FirstOrDefault(s => s.Id == id);
        if (status == null)
        {
            return NotFound();
        }

        status.Content = content;
        status.UpdatedAt = DateTime.Now;

        _repo.UpdateStatus(status);
        await _repo.SaveChangesAsync();

        return Json(new { success = true, updatedAt = status.UpdatedAt?.ToString("g") });
    }

    [HttpPost]
    public async Task<IActionResult> AddStatus(string content)
    {
        var newStatus = new Status
        {
            Content = content,
            IsCurrent = true
        };

        _repo.AddStatus(newStatus);
        await _repo.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    
}