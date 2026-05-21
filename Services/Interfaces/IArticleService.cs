using FinalTask.DTOs;

namespace FinalTask.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleListItemDto> CreateArticleAsync(ArticleDTO dto);
    }
}
