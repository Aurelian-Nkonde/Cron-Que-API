namespace cron_que.Models;

public class Notification
{
    public Guid Id {get;set;}
    public Guid UserId {get;set;}
    public string Message {get;set;} = string.Empty;
    public DateTime CreatedAt{get;set;}
}