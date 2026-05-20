using System.ComponentModel.DataAnnotations;

namespace FinalTask.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // What will be shown in URL address like -> https://mysite.com/categories/sport sport is SLUG
        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        public List<Article> Articles { get; set; } = new();
    }
}
