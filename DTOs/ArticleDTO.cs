using System.ComponentModel.DataAnnotations;

namespace FinalTask.DTOs
{
    public class ArticleDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Array content can not be empty")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;

        public int CategoryId { get; set; }
    }
    public class ArticleListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
    public class ArticleDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Список коментарів, які ми витягнемо з бази для цієї статті
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
    }
}
