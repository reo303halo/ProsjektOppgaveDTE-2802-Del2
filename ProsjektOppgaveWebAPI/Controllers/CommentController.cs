using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
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
    public async Task<IActionResult> Create([FromBody] Comment comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _service.Save(comment, User);
        return CreatedAtAction("GetComment", new { id = comment.PostId }, comment);
    }
    
    
    [Authorize]
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Comment comment)
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

        _service.Save(comment, User);

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

        _service.Delete(id, User);

        return NoContent();
    }
}