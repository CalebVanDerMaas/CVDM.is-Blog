using CVDMBlog.Data.Repository;
using CVDMBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CVDMBlog.Controllers;

public class PostController : Controller
{
    private readonly IRepository _repo;
    private readonly UserManager<IdentityUser> _userManager;

    public PostController(IRepository repo, UserManager<IdentityUser> userManager)
    {
        _repo = repo;
        _userManager = userManager;
    }

    // Existing actions...

    [HttpPost]
    public async Task<IActionResult> AddComment(int id, string content)
    {
        // Check for empty comment
        if (string.IsNullOrWhiteSpace(content))
        {
            // Redirect back to the post without adding the comment
            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return RedirectToAction("Post", "Home", new { slug = post.Slug });
        }

        if (!User.Identity.IsAuthenticated)
        {
            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            string returnUrl = Url.Action("Post", "Home", new { slug = post.Slug });

            return RedirectToAction("Login", "Auth", new
            {
                returnUrl,
                content
            });
        }
        else
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var post = _repo.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Content = content,
                UserId = user.Id,
                PostId = id
            };

            _repo.AddComment(comment);
            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", "Home", new { slug = post.Slug });
        }
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditComment(int commentId, string content)
    {
        var comment = _repo.GetComment(commentId);
        if (comment == null || comment.UserId != _userManager.GetUserId(User))
        {
            return Unauthorized();
        }

        comment.Content = content;
        comment.EditedAt = DateTime.Now;
        _repo.UpdateComment(comment);
        await _repo.SaveChangesAsync();

        return Json(new { 
            success = true, 
            editedAt = comment.EditedAt?.ToString("g")
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comment = _repo.GetComment(commentId);
        if (comment == null || (comment.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin")))
        {
            return Unauthorized();
        }

        _repo.RemoveComment(commentId);
        await _repo.SaveChangesAsync();

        return Json(new { success = true });
    } 
}