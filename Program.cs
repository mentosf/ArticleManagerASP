using FinalTask.Data;
using FinalTask.Middlewares;
using FinalTask.Services;
using FinalTask.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinalTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ArticleDbContext>(options => options.UseNpgsql(connectionString));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie().AddOpenIdConnect(options =>
            {
                options.Authority = "http://localhost:8080/realms/ArticleManager";
                options.ClientId = "mvc-client";
                options.ClientSecret = "63RmkTQYgF3OGgbvUXAU3USIQxVbpcuY";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.RequireHttpsMetadata = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };
                options.Events = new OpenIdConnectEvents
                {
                    //OnTokenValidated = context =>
                    //{
                    //    var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                    //    if(identity != null)
                    //    {
                    //        var roleClaims = identity.FindAll("roles").ToList();
                    //        foreach(var role in roleClaims)
                    //        {
                    //            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role.Value));
                    //        }
                    //    }
                    //    return Task.CompletedTask;
                    //}
                };
            });
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapAreaControllerRoute(
    name: "admin_area",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}"
);
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Article}/{action=Index}/{id?}")
                .WithStaticAssets();




            Console.WriteLine($"\n [EF CORE CONNECTION STRING]: {builder.Configuration.GetConnectionString("DefaultConnection")}\n");
            app.Run();
        }
    }
}
