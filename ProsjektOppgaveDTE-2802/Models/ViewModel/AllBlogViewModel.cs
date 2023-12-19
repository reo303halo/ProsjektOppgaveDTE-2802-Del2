namespace ProsjektOppgaveDTE_2802.Models.ViewModel;

public class AllBlogViewModel
{
    public int BlogId { get; set; }
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public string OwnerEmail { get; set; }
    public int Status { get; set; }
    public ICollection<BlogTagRelations> BlogTags { get; set; }
}