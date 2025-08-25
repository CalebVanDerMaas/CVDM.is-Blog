using System.ComponentModel.DataAnnotations;

namespace CVDMBlog.ViewModels;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string? ReturnUrl { get; set; }
    public string? Content { get; set; }
}