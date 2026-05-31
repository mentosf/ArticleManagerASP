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
        [Authorize(Roles = "Writer,Admin")]
        public IActionResult Secret()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Writer,Admin")]
        public IActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create(ArticleDTO dto)
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
        public IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Update(ArticleDTO dto, int articleId)
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
        public async Task<IActionResult> Delete(int articleId)
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
