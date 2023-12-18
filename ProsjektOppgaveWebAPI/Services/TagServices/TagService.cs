using Microsoft.AspNetCore.Identity;
using ProsjektOppgaveWebAPI.Data;
using ProsjektOppgaveWebAPI.Models;

namespace ProsjektOppgaveWebAPI.Services.TagServices;

public class TagService : ITagService
{
    private readonly BlogDbContext _db;
    private readonly UserManager<IdentityUser> _manager;
    
    public TagService(UserManager<IdentityUser> userManager, BlogDbContext db)
    {
        _manager = userManager;
        _db = db;
    }

    
    // Tag
    public Tag? GetTag(int id)
    {
        var t = (from tag in _db.Tag
                where tag.Id == id
                select tag)
            .FirstOrDefault();

        return t;
    }
    
    public async Task Save(Tag tag)
    {
        var existingTag = _db.Tag.Find(tag.Id);
        if (existingTag == null)
        {
            _db.Tag.Add(tag);
            await _db.SaveChangesAsync();
        }
    }
    
    
    // Relation
    /*
    public BlogTagRelations? GetRelation(int blogId)
    {
        
    }
    */
}