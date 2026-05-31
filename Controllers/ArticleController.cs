using FinalTask.Models;
using FinalTask.DTOs;
using FinalTask.Services.Interfaces;
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
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllPublishedArticlesAsync();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetArticleDetailsAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddComment(string text, int articleId)
        {
            if (string.IsNullOrEmpty(text))
            {
                return RedirectToAction("Details", new { id = articleId });
            }

            await _articleService.CreateCommentAsync(text, articleId);
            return RedirectToAction("Details", new { id = articleId });
        }

        public async Task<IActionResult> AuthorProfile(string authorId)
        {
            if (string.IsNullOrEmpty(authorId)) return NotFound();

            var articles = await _articleService.GetArticlesByAuthorAsync(authorId);

            // Передаємо Id або ім'я автора через ViewBag, щоб на сторінці написати "Профіль автора X"
            ViewBag.AuthorName = articles.FirstOrDefault()?.AuthorName ?? "Автор";

            return View(articles);
        }


        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> MyProfile()
        {
            // Беремо ID поточного залогіненого юзера з Keycloak claims
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myArticles = await _articleService.GetArticlesByAuthorAsync(currentUserId);
            return View(myArticles);
        }



        [Authorize(Roles = "Writer,Admin")]
        public IActionResult Secret()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Writer")]
        public IActionResult CreateArticle()
        {
            return View();
        } 

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateArticle(ArticleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _articleService.CreateArticleAsync(dto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Writer,Admin")]
        public IActionResult UpdateArticle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UpdateArticle(ArticleDTO dto, int articleId)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var result = await _articleService.UpdateArticleAsync(articleId, dto);

            if (result == null)
            {
                return NotFound();
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {

            bool result = await _articleService.DeleteCommentAsync(commentId);

            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {

            bool result = await _articleService.DeleteArticleAsync(articleId);

            if (!result)
            {
                return NotFound();
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _articleService.CreateCategoryAsync(dto);
            return RedirectToAction("Index");
        }


        //[Authorize(Roles = "Reader,Writer,Admin")]
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
