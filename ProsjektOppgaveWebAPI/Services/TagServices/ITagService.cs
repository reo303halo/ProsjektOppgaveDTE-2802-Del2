using ProsjektOppgaveWebAPI.Models;

namespace ProsjektOppgaveWebAPI.Services.TagServices;

public interface ITagService
{
    Tag? GetTag(int id);
    
    Task Save(Tag tag);

    //BlogTagRelations? GetRelation(int blogId);
}