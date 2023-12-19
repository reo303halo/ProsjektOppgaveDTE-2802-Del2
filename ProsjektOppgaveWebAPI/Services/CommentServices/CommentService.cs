using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProsjektOppgaveWebAPI.Data;
using ProsjektOppgaveWebAPI.Models;

namespace ProsjektOppgaveWebAPI.Services.CommentServices;

public class CommentService : ICommentService
{
    private readonly BlogDbContext _db;
    private readonly UserManager<IdentityUser> _manager;
    
    public CommentService(UserManager<IdentityUser> userManager, BlogDbContext db)
    {
        _manager = userManager;
        _db = db;
    }
    
    public async Task<IEnumerable<Comment>> GetCommentsForPost(int postId)
    {
        try
        {
            var comments = _db.Comment
                .Where(c => c.PostId == postId)
                .Include(c => c.Owner)
                .ToList();

            return comments;
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        
            return new List<Comment>();
        }
    }

    public Comment? GetComment(int id)
    {
        var c = (from comment in _db.Comment
                where comment.CommentId == id
                select comment)
            .FirstOrDefault();
        return c;
    }
    
    public async Task Save(Comment comment, string userId)
    {
        var user = await _manager.FindByIdAsync(userId);

        var existingComment = _db.Comment.Find(comment.CommentId);
        if (existingComment != null)
        {
            if (existingComment.Owner != user)
            {
                throw new UnauthorizedAccessException("You are not the owner of this comment.");
            }
            _db.Entry(existingComment).State = EntityState.Detached;
        }

        comment.Owner = user;
        _db.Comment.Update(comment);
        _db.SaveChanges();
    }
    
    public async Task Delete(int id, string userId)
    {
        var user = await _manager.FindByIdAsync(userId);
        var comment = _db.Comment.Find(id);
        
        if (comment.Owner == user)
        {
            _db.Comment.Remove(comment);
            _db.SaveChanges();
        }
        else
        {
            throw new UnauthorizedAccessException("You are not the owner of this post.");
        }
    }
}