using ChatAppWithSignalR.Data;
using ChatAppWithSignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppWithSignalR.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetUserName(string userName)
        {
            var user = new User()
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId
            };
            UserSource.Users.Add(user);
            await Clients.Others.SendAsync("userJoined", userName);
            await Clients.All.SendAsync("users", UserSource.Users);
        }
    }
}
