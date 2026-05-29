using Microsoft.EntityFrameworkCore;
using FinalTask.Models;
namespace FinalTask.Data
{
    public class ArticleDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<Category> Categorys { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=ASP_FinalTask;Username=it_step_user;Password=bot15");
        }
    }
}
