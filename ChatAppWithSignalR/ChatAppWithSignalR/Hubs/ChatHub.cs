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

        public async Task SendMessageAsync(string message, string userName)
        {
            var senderUser = UserSource.Users.FirstOrDefault(u => u.ConnectionId.Equals(Context.ConnectionId));
            if (userName.Trim() == "-1")
                await Clients.Others.SendAsync("receiveMessage", message, senderUser.UserName);
            else
            {
                var user = UserSource.Users.FirstOrDefault(u => u.UserName.Equals(userName));
                await Clients.Client(user.ConnectionId).SendAsync("receiveMessage", message, senderUser.UserName);
            }
        }
    }
}
