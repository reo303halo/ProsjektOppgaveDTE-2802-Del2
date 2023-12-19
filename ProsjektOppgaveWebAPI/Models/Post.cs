using Microsoft.AspNetCore.Identity;

namespace ProsjektOppgaveWebAPI.Models;

public class Post
{
    public int PostId { get; init; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string OwnerId { get; init; }
    public IdentityUser Owner { get; set; }
    public int BlogId { get; init; }
    public Blog Blog { get; set; }
}