using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
using ProsjektOppgaveWebAPI.Models.ViewModel;
using ProsjektOppgaveWebAPI.Services.TagServices;

namespace ProsjektOppgaveWebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly ITagService _service;

    public TagController(ITagService service)
    {
        _service = service;
    }


    [HttpGet("{name}")]
    public Tag? GetTag([FromRoute] string name)
    {
        return _service.GetTag(name.ToLower());
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagViewModel tag)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var existingTag = _service.GetTag(tag.Name);
        if (existingTag != null)
        {
            return new JsonResult(existingTag);
        }

        var newTag = new Tag
        {
            Name = tag.Name.ToLower()
        };
        
        await _service.Save(newTag);
        return CreatedAtAction("GetTag", new { name = tag.Name }, newTag);
    }

    [Authorize]
    [HttpPost("Relation")]
    public async Task<IActionResult> CreateRelation([FromBody] BlogTagRelationsViewModel relation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newRelation = new BlogTagRelations
        {
            BlogId = relation.BlogId,
            TagId = relation.TagId
        };
        
        await _service.SaveRelation(newRelation);
        return Ok("Relation successful!");
    }
}