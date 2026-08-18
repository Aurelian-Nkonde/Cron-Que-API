using cron_que.Dtos;
using cron_que.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cron_que.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService notificationsService;

    public NotificationsController(NotificationsService notificationsService)
    {
        this.notificationsService = notificationsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        return Ok(await notificationsService.GetAllNotificationsAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification(CreateNotificationDto dto)
    {
        var notification = await notificationsService.CreateNotificationAsync(dto);
        return CreatedAtAction(nameof(GetAllNotifications), new { }, notification);
    }
}
