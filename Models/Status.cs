using System;
using System.ComponentModel.DataAnnotations;

namespace CVDMBlog.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsCurrent { get; set; }
    }
}