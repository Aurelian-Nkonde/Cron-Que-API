using cron_que.Models;

namespace cron_que.Dtos;

public record ItemDto(Guid Id, string Name,string ImageUrl ,string Description, Guid UserId, Status Status, DateTime CreatedAt, DateTime ExpiresAt);

public record CreateItemDto(string Name,string ImageUrl,string Description, Guid UserId, string Times);

public record UpdateItemStatusDto(Status Status);

public enum Times
{
    TwoMinutes,
    FiveMinutes,
    FifteenMinutes,
}
