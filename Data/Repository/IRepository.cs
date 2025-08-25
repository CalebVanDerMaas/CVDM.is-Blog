using CVDMBlog.Models;
namespace CVDMBlog.Data.Repository;

public interface IRepository
{
    Post GetPost(string slug);
    Post GetPostById(int id);
    List<Post> GetAllPosts();
    void AddPost(Post post);
    void UpdatePost(Post post);
    void RemovePost(string slug);
    void AddComment(Comment comment);
    void UpdateComment(Comment comment);
    void RemoveComment(int commentId);
    Comment GetComment(int commentId);
    Task<bool> SaveChangesAsync();
    Status GetCurrentStatus();
    void AddStatus(Status status);
    void UpdateStatus(Status status);
    List<Status> GetAllStatuses();
}