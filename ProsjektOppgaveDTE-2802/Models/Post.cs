using Microsoft.AspNetCore.Identity;
using ProsjektOppgaveDTE_2802.Models;

namespace ProsjektOppgaveDTE_2802.Models;

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string OwnerId { get; set; }
    public IdentityUser Owner { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}