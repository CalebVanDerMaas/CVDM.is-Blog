using System.ComponentModel.DataAnnotations;

namespace CVDMBlog.Models;

public class Post
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    [Required]
    [MaxLength(100)]
    public string Slug { get; set; }
    public string Body { get; set; } = "";

    public string Image { get; set; } = "";
    
    public DateTime Created { get; set; } = DateTime.Now;

    public Post()
    {
        Slug = GenerateSlug(Title);
    }
    private string GenerateSlug(string title)
    {
        string slug = title.ToLowerInvariant();
        
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", " ").Trim();
        
        slug = slug.Replace(" ", "-");

        return slug;
    }

    public void UpdateSlug()
    {
        Slug = GenerateSlug(Title);
    }

    public List<Comment> Comments { get; set; } = new List<Comment>(); // Navigation property for comments
}