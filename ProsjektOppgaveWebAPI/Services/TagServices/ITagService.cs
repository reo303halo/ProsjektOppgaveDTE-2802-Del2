using ProsjektOppgaveWebAPI.Models;

namespace ProsjektOppgaveWebAPI.Services.TagServices;

public interface ITagService
{
    Tag? GetTag(string? name);
    
    Task Save(Tag tag);

    Task SaveRelation(BlogTagRelations relation);
}