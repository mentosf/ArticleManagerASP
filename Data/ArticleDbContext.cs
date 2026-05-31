using Microsoft.EntityFrameworkCore;
using FinalTask.Models;
namespace FinalTask.Data
{
    public class ArticleDbContext : DbContext
    {
        //public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options)
        //{
        //}
        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<Category> Categorys { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        
    }
}
