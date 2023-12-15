using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services.CommentServices;

namespace ProsjektOppgaveWebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }
    
    
    [HttpGet]
    public async Task<IEnumerable<Comment>> GetComments(int postId)
    {
        return await _service.GetCommentsForPost(postId);
    }
    
    
    [HttpGet("{id:int}")]
    public Comment? GetComment([FromRoute] int id)
    {
        return _service.GetComment(id);
    }
    
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentViewModel comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newComment = new Comment
        {
            Text = comment.Text,
            PostId = comment.PostId
        };
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        await _service.Save(newComment, userId);
        return CreatedAtAction("GetComment", new { id = comment.PostId }, newComment);
    }
    
    
    [Authorize]
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CommentViewModel comment)
    {
        if (id != comment.CommentId)
            return BadRequest();

        var existingComment = _service.GetComment(id);
        if (existingComment is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (existingComment.OwnerId != userId)
        {
            return Unauthorized();
        }
        
        var newComment = new Comment
        {
            CommentId = comment.CommentId,
            Text = comment.Text,
            PostId = comment.PostId
        };
        
        _service.Save(newComment, userId);
        return NoContent();
    }
    
    
    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var comment = _service.GetComment(id);
        if (comment is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (comment.OwnerId != userId)
        {
            return Unauthorized();
        }

        _service.Delete(id, userId);
        return NoContent();
    }
}