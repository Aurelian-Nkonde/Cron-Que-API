

using cron_que.Models;
using Microsoft.EntityFrameworkCore;

namespace cron_que.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

    public DbSet<User> Users {get;set;}
    public DbSet<Like> Likes {get;set;}
    public DbSet<Item> Items {get;set;}
    public DbSet<Notification> Notifications {get;set;}
}