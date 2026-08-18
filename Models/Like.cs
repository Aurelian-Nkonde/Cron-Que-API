namespace cron_que.Models;

public class Like
{
    public Guid Id {get;set;}
    public Guid UserId {get;set;}
    public Guid ItemId {get;set;}
    public DateTime CreatedAt{get;set;}
}