namespace FinalTask.Models
{
    public class FavoriteArticle
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ArticleId { get; set; }

        
        public Article Article { get; set; } = null!;
    }
}
