using CVDMBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace CVDMBlog.Data.Repository;

public class Repository : IRepository
{
    private AppDbContext _ctx;
    public Repository(AppDbContext ctx)
    {
        _ctx = ctx;
    }
    public Post GetPost(string slug)
    {
        return _ctx.Posts
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefault(p => p.Slug == slug);
    }

    public Post GetPostById(int id)
    {
        return _ctx.Posts.FirstOrDefault(p => p.Id == id);
    }

    public List<Post> GetAllPosts()
    {
        return _ctx.Posts.ToList();
    }

    public void AddPost(Post post)
    {
        _ctx.Posts.Add(post);
        
    }

    public void UpdatePost(Post post)
    {
        _ctx.Posts.Update(post);
    }

    public void RemovePost(string slug)
    {
        _ctx.Posts.Remove(GetPost(slug));
    }
    
    public void AddComment(Comment comment)
    {
        _ctx.Comments.Add(comment);
    }

    public void UpdateComment(Comment comment)
    {
        _ctx.Comments.Update(comment);
    }

    public void RemoveComment(int commentId)
    {
        var comment = GetComment(commentId);
        if (comment != null)
        {
            _ctx.Comments.Remove(comment);
        }
    }

    public Comment GetComment(int commentId)
    {
        return _ctx.Comments.FirstOrDefault(c => c.Id == commentId);
    }
    public async Task<bool> SaveChangesAsync()
    {
        if (await _ctx.SaveChangesAsync() > 0)
        {
            return true;
        }
        return false;
    }
    
    public Status GetCurrentStatus()
    {
        return _ctx.Statuses.FirstOrDefault(s => s.IsCurrent);
    }

    public void AddStatus(Status status)
    {
        if (status.IsCurrent)
        {
            var currentStatus = GetCurrentStatus();
            if (currentStatus != null)
            {
                currentStatus.IsCurrent = false;
                _ctx.Statuses.Update(currentStatus);
            }
        }
        status.IsCurrent = true;
        _ctx.Statuses.Add(status);
    }

    public void UpdateStatus(Status status)
    {
        if (status.IsCurrent)
        {
            var currentStatus = GetCurrentStatus();
            if (currentStatus != null && currentStatus.Id != status.Id)
            {
                currentStatus.IsCurrent = false;
                _ctx.Statuses.Update(currentStatus);
            }
        }
        status.UpdatedAt = DateTime.Now;
        _ctx.Statuses.Update(status);
    }

    public List<Status> GetAllStatuses()
    {
        return _ctx.Statuses.OrderByDescending(s => s.CreatedAt).ToList();
    }
}