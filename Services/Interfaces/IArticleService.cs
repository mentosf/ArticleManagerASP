using FinalTask.DTOs;

namespace FinalTask.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleListItemDto> CreateArticleAsync(ArticleDTO dto);
        Task<ArticleListItemDto> UpdateArticleAsync(int articleId, ArticleDTO dto);
        Task<bool> DeleteArticleAsync(int articleId);
        Task<CommentDTO> CreateCommentAsync(string text, int articleId);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto);
        Task<IEnumerable<ArticleListItemDto>> GetAllPublishedArticlesAsync(string? search = null, string? category = null);
        Task<ArticleDetailsDto> GetArticleDetailsAsync(int articleId);
        Task<IEnumerable<ArticleListItemDto>> GetArticlesByAuthorAsync(string authorId);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<ArticleDTO> GetArticleForEditAsync(int articleId);
        Task<bool> ToggleFavoriteAsync(int articleId, string userId);
        Task<IEnumerable<ArticleListItemDto>> GetFavoriteArticlesAsync(string userId);
    }
}
