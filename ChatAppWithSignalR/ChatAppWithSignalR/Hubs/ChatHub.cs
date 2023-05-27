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

        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = new Group()
            {
                GroupName = groupName
            };
            group.Users.Add(UserSource.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId));
            GroupSource.Groups.Add(group);
            await Clients.All.SendAsync("groups", GroupSource.Groups);
        }

        public async Task AddClientToGroup(string groupName)
        {
            var group = GroupSource.Groups.FirstOrDefault(g => g.GroupName.Equals(groupName));
            var user = UserSource.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var result = group.Users.Any(u => u.ConnectionId == user.ConnectionId);
            if (!result)
            {
                group.Users.Add(user);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task GetClientToGroup(string groupName)
        {
            var group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == groupName);
            await Clients.Caller.SendAsync("users", groupName == "-1" ? UserSource.Users : group.Users);
        }
    }
}
