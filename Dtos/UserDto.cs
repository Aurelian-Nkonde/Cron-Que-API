namespace cron_que.Dtos;

public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);

public record CreateUserDto(string Name, string Email, string Password);
