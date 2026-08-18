using cron_que.Data;
using cron_que.Dtos;
using cron_que.Models;
using Microsoft.EntityFrameworkCore;

namespace cron_que.Service;

public class LikesService
{
    private readonly AppDbContext db;

    public LikesService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<LikeDto> CreateLikeAsync(CreateLikeDto dto)
    {
        var like = new Like
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ItemId = dto.ItemId,
            CreatedAt = DateTime.UtcNow,
        };

        db.Likes.Add(like);
        await db.SaveChangesAsync();

        return new LikeDto(like.Id, like.UserId, like.ItemId, like.CreatedAt);
    }

    public async Task<List<LikeDto>> GetAllLikesAsync()
    {
        return await db.Likes
            .Select(l => new LikeDto(l.Id, l.UserId, l.ItemId, l.CreatedAt))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<LikeDto?> GetLikeByIdAsync(Guid id)
    {
        var like = await db.Likes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        return like is null ? null : new LikeDto(like.Id, like.UserId, like.ItemId, like.CreatedAt);
    }

    public async Task<List<LikeDto>> GetAllLikesForItemAsync(Guid id)
    {
        return await db.Likes
            .Where(l => l.ItemId == id)
            .Select(l => new LikeDto(l.Id, l.UserId, l.ItemId, l.CreatedAt))
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<bool> DeleteLikeAsync(Guid id)
    {
        var like = await db.Likes.FindAsync(id);
        if (like is null)
        {
            return false;
        }

        db.Likes.Remove(like);
        await db.SaveChangesAsync();

        return true;
    }
}
