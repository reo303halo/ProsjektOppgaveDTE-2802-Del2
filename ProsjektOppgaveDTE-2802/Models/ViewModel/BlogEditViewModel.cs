using System.ComponentModel.DataAnnotations;

namespace ProsjektOppgaveDTE_2802.Models.ViewModel;

public class BlogViewModel
{
    public int BlogId { get; set; }
    [Required(ErrorMessage = "Blog Name Required")]
    public string Name { get; set; }
    public int Status { get; set; }
}