using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Graduation.Interfaces;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;
using Graduation.Data;

namespace Graduation.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private const int MinutesToUpdate = 1;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userIdString = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null || !int.TryParse(userIdString, out int userId)) return;

            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            if (repo == null) return;

            var user = await repo.GetUserByIdAsync(userId);
            if (user == null) return;

            await ((DataContext)context.HttpContext.RequestServices.GetRequiredService(typeof(DataContext))).Entry(user).ReloadAsync();

            var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time") 
                ?? TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
            var cairoTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cairoTimeZone);

            if (cairoTime.Subtract(user.LastActive).TotalMinutes >= MinutesToUpdate)
            {
                user.LastActive = cairoTime;
                await repo.SaveAllAsync();
            }
        }
    }
}