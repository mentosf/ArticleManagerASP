using FinalTask.Data;
using FinalTask.DTOs;
using FinalTask.Models;
using Microsoft.EntityFrameworkCore;
using FinalTask.Services.Interfaces;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;
namespace FinalTask.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ArticleDbContext _db;
        public ArticleService(ArticleDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
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
            _db.Articles.Add(article);

            await _db.SaveChangesAsync();
            
            return new ArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                CreatedAt = article.CreatedAt,
                AuthorName = article.AuthorName
            };
        }
        public async Task<ArticleListItemDto> UpdateArticleAsync(int articleId, ArticleDTO newDto)
        {
            var exactArticle = await _db.Articles.FirstOrDefaultAsync(s => s.Id == articleId);
            if (exactArticle != null)
            {
                exactArticle.Title = newDto.Title;
                exactArticle.Content = newDto.Content;
                exactArticle.Summary = newDto.Summary;
                exactArticle.CategoryId = newDto.CategoryId;
                await _db.SaveChangesAsync();
            }
            else
            {
                return null;
            }

            return new ArticleListItemDto
            {
                Id = exactArticle.Id,
                Title = exactArticle.Title,
                Summary = exactArticle.Summary,
                CreatedAt = exactArticle.CreatedAt,
                AuthorName = exactArticle.AuthorName
            };


        }

        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            var exactArticle = await _db.Articles.FirstOrDefaultAsync(s => s.Id == articleId);
            if (exactArticle != null)
            {
                _db.Remove(exactArticle);
                await _db.SaveChangesAsync();

            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<CommentDTO> CreateCommentAsync(string text, int articleId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                Console.WriteLine("UnAuthorized access");
                return null;
            }
            var comment = new Comment
            {
                Text = text,
                CreatedAt = DateTime.UtcNow,
                ArticleId = articleId,
                Username = user.Identity?.Name ?? "Anonym",
                UserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();


            return new CommentDTO
            {
                Text = comment.Text,
                CreatedAt = comment.CreatedAt,
                ArticleId = comment.ArticleId,
                Username = comment.Username,
                UserId = comment.UserId,
            };
        
        }
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var exactComment = _db.Comments.FirstOrDefaultAsync(s => s.Id == commentId);
            if (exactComment != null)
            {
                _db.Remove(exactComment);
                await _db.SaveChangesAsync();

            }
            else
            {
                return false;
            }
            return true;
        }


        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO dto)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                Console.WriteLine("UnAuthorized access");
                return null;
            }
            var category = new Category
            {
                Name = dto.Name,
                Slug = dto.Slug
            };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();


            return new CategoryDTO
            {
                Name = dto.Name,
                Slug = dto.Slug
            };

        }
        public async Task<IEnumerable<ArticleListItemDto>> GetAllPublishedArticlesAsync(string? search = null, string? category = null)
        {
            var query = _db.Articles.AsQueryable();

            // Фільтрація за категорією (через Slug)
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category.Slug == category);
            }

            // Пошук за назвою або вмістом
            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(lowerSearch) ||
                                         a.Summary.ToLower().Contains(lowerSearch));
            }

            return await query
                .Include(a => a.Category)
                .Select(a => new ArticleListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Summary = a.Summary,
                    CreatedAt = a.CreatedAt,
                    AuthorName = a.AuthorName
                }).ToListAsync();
        }
        public async Task<ArticleDetailsDto> GetArticleDetailsAsync(int articleId)
        {
            var article = await _db.Articles
                .Include(a => a.Category)
                .Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Id == articleId);

            if (article == null)
            {
                return null;
            }

            return new ArticleDetailsDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Summary = article.Summary,
                CreatedAt = article.CreatedAt,
                AuthorName = article.AuthorName,
                CategoryName = article.Category?.Name ?? "No category",
                AuthorId = article.AuthorId,
                Comments = article.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    Username = c.Username,
                    UserId = c.UserId,
                    ArticleId = c.ArticleId
                }).ToList()
            };
        }
        public async Task<IEnumerable<ArticleListItemDto>> GetArticlesByAuthorAsync(string authorId)
        {
            return await _db.Articles
                .Where(a => a.AuthorId == authorId)
                .Select(a => new ArticleListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Summary = a.Summary,
                    CreatedAt = a.CreatedAt,
                    AuthorName = a.AuthorName
                }).ToListAsync();
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return await _db.Categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug
                }).ToListAsync();
        }
        public async Task<ArticleDTO> GetArticleForEditAsync(int articleId)
        {
            var article = await _db.Articles.FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null) return null!;

            return new ArticleDTO
            {
                Id = article.Id, 
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                CategoryId = article.CategoryId
            };
        }

        public async Task<bool> ToggleFavoriteAsync(int articleId, string userId)
        {
            var existing = await _db.FavoriteArticles
                .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId);

            if (existing != null)
            {
                _db.FavoriteArticles.Remove(existing);
                await _db.SaveChangesAsync();
                return false; // Видалено
            }

            var favorite = new FavoriteArticle { ArticleId = articleId, UserId = userId };
            _db.FavoriteArticles.Add(favorite);
            await _db.SaveChangesAsync();
            return true; // Додано
        }

        // Отримати всі збережені статті користувача
        public async Task<IEnumerable<ArticleListItemDto>> GetFavoriteArticlesAsync(string userId)
        {
            return await _db.FavoriteArticles
                .Where(f => f.UserId == userId)
                .Select(f => f.Article) // Переходимо до самої статті
                .Select(a => new ArticleListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Summary = a.Summary,
                    CreatedAt = a.CreatedAt,
                    AuthorName = a.AuthorName
                }).ToListAsync();
        }
    }

}
