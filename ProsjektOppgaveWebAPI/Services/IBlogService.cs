using System.Security.Principal;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;

namespace ProsjektOppgaveWebAPI.Services;

public interface IBlogService
{
    // Blog
    Task<IEnumerable<AllBlogViewModel>> GetAllBlogs();

    Blog? GetBlog(int id);
    
    Task Save(Blog blog, string userId);
    
    Task Delete(int id , string userId);

    
    // Post
    Task<IEnumerable<Post>> GetPostsForBlog(int blogId);

    Post? GetPost(int id);
    
    Task SavePost(Post post, string userId);

    Task DeletePost(int id, string userId);
}