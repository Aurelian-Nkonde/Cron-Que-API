namespace cron_que.Dtos;

public record LikeDto(Guid Id, Guid UserId, Guid ItemId, DateTime CreatedAt);

public record CreateLikeDto(Guid UserId, Guid ItemId);
