using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProsjektOppgaveWebAPI.Models;
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


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tag tag)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _service.Save(tag);

        return Ok();
    }
}