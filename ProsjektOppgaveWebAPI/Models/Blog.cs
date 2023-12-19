using Microsoft.AspNetCore.Identity;

namespace ProsjektOppgaveWebAPI.Models;

public class Blog
{
    public int BlogId { get; init; }
    public string Name { get; init; }
    public string OwnerId { get; init; }
    public IdentityUser Owner { get; set; }
    public List<Post> Posts { get; set; }
    public int Status { get; init; }
    public ICollection<BlogTagRelations> BlogTags { get; }
}