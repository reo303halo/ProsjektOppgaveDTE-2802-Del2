namespace ProsjektOppgaveWebAPI.Models;

public class BlogTagRelations
{
    public int BlogId { get; init; }
    public int TagId { get; init; }
    public Tag Tag { get; }
}