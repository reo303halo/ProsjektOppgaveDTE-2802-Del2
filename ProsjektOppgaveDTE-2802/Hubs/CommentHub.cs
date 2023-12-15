using Microsoft.AspNetCore.SignalR;
using ProsjektOppgaveDTE_2802.Models;

namespace ProsjektOppgaveDTE_2802.Hubs;

public class CommentHub : Hub
{
    public async Task CommentAdded(Comment comment)
    {
        await Clients.All.SendAsync("CommentAdded", comment);
    }

    public async Task CommentDeleted(int commentId)
    {
        await Clients.All.SendAsync("CommentDeleted", commentId);
    }
}
