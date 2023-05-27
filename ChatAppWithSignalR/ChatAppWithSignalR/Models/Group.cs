namespace ChatAppWithSignalR.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<User> Users { get; } = new List<User>();
    }
}
