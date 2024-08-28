using CVDMBlog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CVDMBlog.Data;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        :base(options)
    {
        
    }
    public DbSet<Post> Posts { get; set; }
}