using System.Security.Principal;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;

namespace ProsjektOppgaveWebAPI.Services.CommentServices;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetCommentsForPost(int postId);

    Comment? GetComment(int id);
    
    Task Save(Comment comment, string userId);

    Task Delete(int id, string userId);
}