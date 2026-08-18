namespace cron_que.Dtos;

public record NotificationDto(Guid Id, Guid UserId, string Message, DateTime CreatedAt);

public record CreateNotificationDto(Guid UserId, string Message);
