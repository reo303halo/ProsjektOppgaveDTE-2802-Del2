using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services;

namespace ProsjektOppgaveWebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly IBlogService _service;
    
    public BlogController(IBlogService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IEnumerable<AllBlogViewModel>> GetAll()
    {
        return await _service.GetAllBlogs();
    }


    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var blog = _service.GetBlog(id);
        if (blog == null)
        {
            return NotFound();
        }
        return Ok(blog);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BlogViewModel blog)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newBlog = new Blog
        {
            Name = blog.Name,
            Status = blog.Status
        };
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) await _service.Save(newBlog, userId);
        return CreatedAtAction("Get", new { id = blog.BlogId }, newBlog);
    }


    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BlogViewModel blog)
    {
        if (id != blog.BlogId)
            return BadRequest();

        var existingBlog = _service.GetBlog(id);
        
        if (existingBlog is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (existingBlog.OwnerId != userId)
        {
            return Unauthorized();
        }
        
        var newBlog = new Blog
        {
            BlogId = blog.BlogId,
            Name = blog.Name,
            Status = blog.Status
        };
        
        await _service.Save(newBlog, userId);
        return NoContent();
    }

    
    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var blog = _service.GetBlog(id);
        if (blog is null)
            return NotFound();
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (blog.OwnerId != userId)
        {
            return Unauthorized();
        }

        _service.Delete(id, userId);
        return NoContent();
    }
}