using FinalTask.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalTask.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int ArticleId { get; set; }
        public List<Article> Articles { get; set; } = new();
    }

    
}
