using FinalTask.Models;
using FinalTask.DTOs;
using FinalTask.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalTask.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ArticleService _articleService;

        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Writer")]
        public IActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public IActionResult Create(Article article)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;

            article.AuthorId = userId ?? string.Empty;
            article.AuthorName = username ?? "Anonym";
            article.CreatedAt = DateTime.UtcNow;

            // _context.Articles.Add(article);
            // _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Writer")]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }
        [Authorize]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
