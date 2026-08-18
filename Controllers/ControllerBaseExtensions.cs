using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

public static class ControllerBaseExtensions
{
    public static Guid GetUserId(this ControllerBase controller) =>
        Guid.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
