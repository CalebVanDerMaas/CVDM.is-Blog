using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace CVDMBlog.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = "";

    [Required]
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? EditedAt { get; set; }

    [Required]
    public string UserId { get; set; } // Link to the user who made the comment

    public IdentityUser User { get; set; } // The actual user entity

    [Required]
    public int PostId { get; set; } // Link to the post

    public Post Post { get; set; } // The actual post entity
}