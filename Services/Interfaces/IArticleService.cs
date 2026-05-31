using FinalTask.DTOs;

namespace FinalTask.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleListItemDto> CreateArticleAsync(ArticleDTO dto);
        Task<ArticleListItemDto> UpdateArticleAsync(int articleId, ArticleDTO dto);
        Task<bool> DeleteArticleAsync(int articleId);
        Task<CommentDTO> CreateCommentAsync(string text, int articleId);
    }
}
