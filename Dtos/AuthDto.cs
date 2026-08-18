namespace cron_que.Dtos;

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, DateTime ExpiresAt);
