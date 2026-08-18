using cron_que.Data;
using cron_que.Dtos;
using cron_que.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace cron_que.Service;

public class UsersService
{
    private readonly AppDbContext db;

    public UsersService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            // Password = dto.Password,
            CreatedAt = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await db.Users
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.CreatedAt))
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await db.Users.FindAsync(id);
        return user is null ? null : new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }

        return user;
    }
}
