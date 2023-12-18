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


    [HttpGet("{id:int}")]
    public Tag? GetTag([FromRoute] int id)
    {
        return _service.GetTag(id);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagViewModel tag)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newTag = new Tag
        {
            Name = tag.Name
        };

        await _service.Save(newTag);
        return CreatedAtAction("GetTag", new { id = newTag.Id }, newTag);
    }
}