namespace ProsjektOppgaveWebAPI.Models;

public class BlogTagRelations
{
    public int BlogId { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}