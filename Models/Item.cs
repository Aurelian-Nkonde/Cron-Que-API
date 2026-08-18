namespace cron_que.Models;

public enum Status
{
    ACTIVE,
    EXPIRED,
}

public class Item
{
    public Guid Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public Guid UserId {get;set;}
    public Status Status {get;set;} = Status.ACTIVE;
    public DateTime CreatedAt{get;set;}
    public DateTime ExpiresAt{get;set;}
}