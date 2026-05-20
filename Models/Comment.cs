using System.ComponentModel.DataAnnotations;

namespace FinalTask.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment text can not be empty")]
        [StringLength(1000)]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public int ArticleId { get; set; }
        public Article? Article { get; set; }

        // Who wrote comment (Keycloak UserId)
        [Required]
        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}
