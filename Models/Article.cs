using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace FinalTask.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title can not be over 200 symbols")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Array content can not be empty")]
        public string Content { get; set; } = string.Empty;

        
        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        
        [Required]
        public string AuthorId { get; set; } = string.Empty;

        
        public string AuthorName { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = false;

        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        
        public List<Comment> Comments { get; set; } = new();
    }
}
