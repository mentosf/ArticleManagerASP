using FinalTask.Data;
using FinalTask.DTOs;
using FinalTask.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ArticleDbContext _db;
        private readonly IArticleService _articleService;

        public DashboardController(IArticleService articleService, ArticleDbContext db)
        {
            _articleService = articleService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadDashboardStats(); 
            var allArticles = await _db.Articles
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(allArticles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDashboardStats();
                var allArticles = await _db.Articles.OrderByDescending(a => a.CreatedAt).Take(10).ToListAsync();
                return View("Index", allArticles);
            }

            await _articleService.CreateCategoryAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArticleFromLog(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDashboardStats()
        {
            ViewBag.TotalArticles = await _db.Articles.CountAsync();
            ViewBag.TotalComments = await _db.Comments.CountAsync();
            ViewBag.TotalCategories = await _db.Categories.CountAsync();
            ViewBag.CategoriesList = await _articleService.GetAllCategoriesAsync();
        }
    }
}
