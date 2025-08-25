using System.ComponentModel.DataAnnotations;

namespace CVDMBlog.Models;

public class StatusUpdateModel
{
    [Required]
    public string Content { get; set; }
}