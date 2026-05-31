using FinalTask.Data;
using FinalTask.DTOs;
using FinalTask.Models;
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
            var exactArticle = _db.Articles.FirstOrDefault(s => s.Id == articleId);
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
            var exactArticle = _db.Articles.FirstOrDefault(s => s.Id == articleId);
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
                CreatedAt = DateTime.Now,
                ArticleId = articleId,
                Username = user.Identity?.Name ?? "Anonym",
                UserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();


            return new CommentDTO
            {
                Text = comment.Text,
                CreatedAt = DateTime.Now,
                ArticleId = comment.ArticleId,
                Username = comment.Username,
                UserId = comment.UserId,
            };
        
        }
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var exactComment = _db.Comments.FirstOrDefault(s => s.Id == commentId);
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
    }





}
