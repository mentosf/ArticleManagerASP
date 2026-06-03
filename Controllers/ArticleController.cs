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
        public async Task<IActionResult> Index(string? search, string? category)    
        {
            var articles = await _articleService.GetAllPublishedArticlesAsync(search, category);
            ViewBag.Categories = await _articleService.GetAllCategoriesAsync();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = category;
            // Отримуємо ID всіх обраних статей для поточного користувача
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoriteIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                // Отримуємо статті користувача і забираємо лише їхні Id
                var favorites = await _articleService.GetFavoriteArticlesAsync(userId);
                favoriteIds = favorites.Select(f => f.Id).ToList();
            }

            // Передаємо цей список
            ViewBag.FavoriteIds = favoriteIds;
            return View(articles);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int articleId, string returnUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            await _articleService.ToggleFavoriteAsync(articleId, userId);

            // Повертаємо користувача туди, звідки він клікнув (на головну або в деталку)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
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

        [HttpGet]
        public async Task<IActionResult> AuthorProfile([FromQuery] string? authorId, string? id)
        {
            // Перевіряємо обидва варіанти імені параметра (authorId або id), які могли прийти з маршруту
            string? targetUserId = !string.IsNullOrEmpty(authorId) ? authorId : id;

            // якщо ВЗАГАЛІ нічого не передали в URL (клікнули на "Profile" в шапці) — показуємо свій профіль
            if (string.IsNullOrEmpty(targetUserId))
            {
                targetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (string.IsNullOrEmpty(targetUserId)) return NotFound();

            // Отримуємо статті саме ТОГО автора, чий ID ми отримали
            var articles = await _articleService.GetArticlesByAuthorAsync(targetUserId);

            // Перевіряємо, чи є цей targetUserId автором (має роль Writer)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isTargetWriter = (articles != null && articles.Any()) ||
                                  (targetUserId == currentUserId && User.IsInRole("Writer"));

            if (isTargetWriter)
            {
                ViewBag.ProfileType = "Published Works";
                // Якщо статей немає (новий врайтер), але це мій профіль — пишемо мій нік, інакше "Staff Writer"
                ViewBag.AuthorName = articles?.FirstOrDefault()?.AuthorName ??
                                     (targetUserId == currentUserId ? User.Identity?.Name : "Staff Writer");
            }
            else
            {
                ViewBag.ProfileType = "Saved Bookmarks";
                // Якщо це чужий профіль Рідера/Адміна — показуємо "Platform Contributor", якщо свій — твій нік
                ViewBag.AuthorName = targetUserId == currentUserId ? (User.Identity?.Name ?? "Contributor") : "Platform Contributor";

                // Для рідерів завантажуємо їхнє обране
                articles = await _articleService.GetFavoriteArticlesAsync(targetUserId);
            }

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
        public async Task<IActionResult> CreateArticle()
        {           
            var categories = await _articleService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateArticle(ArticleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _articleService.GetAllCategoriesAsync();
                return View(dto);
            }
            await _articleService.CreateArticleAsync(dto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UpdateArticle(int articleId)
        {
            var dto = await _articleService.GetArticleForEditAsync(articleId);
            if (dto == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _articleService.GetAllCategoriesAsync();

            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> UpdateArticle(ArticleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var result = await _articleService.UpdateArticleAsync(dto.Id, dto);

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
