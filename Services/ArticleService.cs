namespace FinalTask.Services
{
    public class ArticleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
