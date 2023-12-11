using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Services;

namespace ProsjektOppgaveWebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IBlogService _service;

    public PostController(IBlogService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IEnumerable<Post>> GetPosts(int blogId)
    {
        return await _service.GetPostsForBlog(blogId);
    }
    
    
    [HttpGet("{id:int}")]
    public Post? GetPost([FromRoute] int id)
    {
        return _service.GetPost(id);
    }
    
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var blog = _service.GetBlog(post.BlogId);
        if (blog != null && blog.Status != 0) return BadRequest("This blog is closed for new posts and comments!");
        
        await _service.SavePost(post, User);
        return CreatedAtAction("GetPosts", new { id = post.BlogId }, post);
    }

    
    [Authorize]
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] Post post)
    {
        if (id != post.PostId)
            return BadRequest();

        var existingPost = _service.GetPost(id);
        if (existingPost is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (existingPost.OwnerId != userId)
        {
            return Unauthorized();
        }
        
        _service.SavePost(post, User);

        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var post = _service.GetPost(id);
        if (post is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (post.OwnerId != userId)
        {
            return Unauthorized();
        }

        _service.DeletePost(id, User);

        return NoContent();
    }
}