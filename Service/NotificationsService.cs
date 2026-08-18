using cron_que.Data;
using cron_que.Dtos;
using cron_que.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace cron_que.Service;

public class NotificationsService
{
    private readonly AppDbContext db;
    private readonly LikesService _likesService;

    public NotificationsService(AppDbContext db, LikesService likesService)
    {
        this.db = db;
        _likesService = likesService;
    }

    public async Task<List<NotificationDto>> GetAllNotificationsAsync()
    {
        return await db.Notifications
            .Select(n => new NotificationDto(n.Id, n.UserId, n.Message, n.CreatedAt))
            .AsNoTracking()
            .ToListAsync();
    }

    [Queue("general-queue")]
    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow,
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();

        return new NotificationDto(notification.Id, notification.UserId, notification.Message, notification.CreatedAt);
    }


    [Queue("general-queue")]
    public async Task<bool> ExpireItemInformUsersAsync(Guid id)
    {
        var likes = await _likesService.GetAllLikesForItemAsync(id);
        var userIds = likes.Select(l => l.UserId);
        foreach(var user in userIds)
        {
            await CreateNotificationAsync(new CreateNotificationDto(
                UserId: user,
                Message: "An item that you liked has expired!"
            ){});
        }
        return true;
    }
}
