using cron_que.Data;
using cron_que.Dtos;
using cron_que.Models;
using Hangfire;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace cron_que.Service;

public class ItemsService
{
    private readonly AppDbContext db;
    private readonly NotificationsService _notificationsService;

    public ItemsService(AppDbContext db, NotificationsService notificationsService)
    {
        this.db = db;
        _notificationsService = notificationsService;
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemDto dto)
    {
        var times = TimesHelper.ParseTimes(dto.Times);
        var createdAt = DateTime.UtcNow;

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            UserId = dto.UserId,
            ImageUrl = dto.ImageUrl,
            CreatedAt = createdAt,
            ExpiresAt = createdAt.AddMinutes(times.ToMinutes()),
        };

        db.Items.Add(item);
        await db.SaveChangesAsync();

        var notification = new CreateNotificationDto(UserId: item.UserId, Message: "Your item has been created!"){};
        BackgroundJob.Enqueue(() =>  _notificationsService.CreateNotificationAsync(notification));
        BackgroundJob.Schedule(() => ExpireItemAsync(item.Id), TimeSpan.FromMinutes(TimesHelper.ToMinutes(times)));

        return ToDto(item);
    }

    public async Task<List<ItemDto>> GetAllItemsAsync()
    {
        return await db.Items
            .Select(i => new ItemDto(i.Id, i.Name, i.ImageUrl, i.Description, i.UserId, i.Status, i.CreatedAt, i.ExpiresAt))
            .AsNoTracking().ToListAsync();
    }

    public async Task<ItemDto?> GetItemByIdAsync(Guid id)
    {
        var item = await db.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        return item is null ? null : ToDto(item);
    }

    public async Task<ItemDto?> UpdateItemStatusAsync(Guid id, UpdateItemStatusDto dto)
    {
        var item = await db.Items.FindAsync(id);
        if (item is null)
        {
            return null;
        }

        item.Status = dto.Status;
        await db.SaveChangesAsync();

        return ToDto(item);
    }

    [Queue("general-queue")]
    public async Task<ItemDto?> ExpireItemAsync(Guid id)
    {
        var item = await db.Items.FirstOrDefaultAsync(i => i.Id == id) ?? throw new BadHttpRequestException("Item is not found");
        item.Status = Status.EXPIRED;
        await db.SaveChangesAsync();
        var result = new ItemDto(
            Id: item.Id,
            UserId: item.UserId,
            ImageUrl: item.ImageUrl,
            Name: item.Name,
            Status: item.Status,
            Description: item.Description,
            CreatedAt: item.CreatedAt,
            ExpiresAt: item.ExpiresAt
        ){};
        // send notifications 
        BackgroundJob.Enqueue(() => _notificationsService.ExpireItemInformUsersAsync(item.Id));
        return result;
    }


    private static ItemDto ToDto(Item item) =>
        new(item.Id, item.Name, item.ImageUrl, item.Description, item.UserId, item.Status, item.CreatedAt, item.ExpiresAt);

}
