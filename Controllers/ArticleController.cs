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
        public IActionResult Create(ArticleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _articleService.CreateArticleAsync(dto);




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
