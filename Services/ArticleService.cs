using FinalTask.DTOs;
using FinalTask.Models;
using FinalTask.Services.Interfaces;
using System.Security.Claims;
namespace FinalTask.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ArticleListItemDto> CreateArticleAsync(ArticleDTO dto)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                Console.WriteLine("UnAuthorized access");
                return null;
            }
            var article = new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                Summary = dto.Summary,
                CategoryId = dto.CategoryId,


                AuthorId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                AuthorName = user.Identity?.Name ?? "Anonym",
                CreatedAt = DateTime.UtcNow,
                IsPublished = false
            };
            return new ArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                CreatedAt = article.CreatedAt,
                AuthorName = article.AuthorName
            };
        }
    }
}
