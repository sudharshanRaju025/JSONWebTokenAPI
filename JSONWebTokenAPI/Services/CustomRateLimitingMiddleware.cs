using Azure;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

public class CustomRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    private readonly Dictionary<string, int> limits = new Dictionary<string, int>
    {
        { "Admin", 10 },
        { "User", 7 },
        { "Anonymous", 5} 
    };

    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public CustomRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        var roleClaim = user.Claims.FirstOrDefault(c =>
                             c.Type == ClaimTypes.Role ||
                                        c.Type == "Roles");
      
        string role;

        if (roleClaim != null)
        {
            role = roleClaim.Value;
        }
        else
        {
            role = "Anonymous";
        }

        int limit = limits.ContainsKey(role) ? limits[role] : limits["Anonymous"];


        string key;
        bool isAuthenticated = user.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            key = $"{role}-{user.Identity!.Name}";
        }
        else
        {
            key = $"Anonymous-{context.Connection.RemoteIpAddress}";
        }


        var counter = _cache.GetOrCreate(key, entry =>
        {
            entry.SetAbsoluteExpiration(_timeWindow);
            return new RequestCounter { Count = 0 };
        });

        if (counter.Count >= limit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again after 60 seconds...");
            return;
        }

        counter.Count++;
        await _next(context);
    }

    private class RequestCounter
    {
        public int Count { get; set; }
    }
}
