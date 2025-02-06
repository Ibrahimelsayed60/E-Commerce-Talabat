using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSeconds;

        public CachedAttribute(int ExpireTimeInSeconds)
        {
            _expireTimeInSeconds = ExpireTimeInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await CacheService.GetCachedResponse(cacheKey);

            if(!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult() 
                {
                    Content = cachedResponse,
                    ContentType ="application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }

            var ExecutedEndpointContext =await next.Invoke();

            if(ExecutedEndpointContext.Result is OkObjectResult result)
            {
                await CacheService.CacheResponseAsync(cacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSeconds));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);

            foreach(var (key, value) in request.Query.OrderBy(X => X.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
