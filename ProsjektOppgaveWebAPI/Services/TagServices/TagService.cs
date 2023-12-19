using ProsjektOppgaveWebAPI.Data;
using ProsjektOppgaveWebAPI.Models;

namespace ProsjektOppgaveWebAPI.Services.TagServices;

public class TagService : ITagService
{
    private readonly BlogDbContext _db;
    
    public TagService(BlogDbContext db)
    {
        _db = db;
    }

    
    // Tag
    public Tag? GetTag(string? name)
    {
        var t = (from tag in _db.Tag
                where tag.Name == name
                select tag)
            .FirstOrDefault();

        return t;
    }
    
    public async Task Save(Tag tag)
    {
        var existingTag = GetTag(tag.Name);
        if (existingTag == null)
        {
            _db.Tag.Add(tag);
            await _db.SaveChangesAsync();
        }
    }
    
    
    // Relation
    public async Task SaveRelation(BlogTagRelations relation)
    {
        _db.BlogTagRelations.Add(relation);
        await _db.SaveChangesAsync();
    }
}