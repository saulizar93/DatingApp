namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        // by convention, if we name the Id the same as the class name ex: ConnectionId, EntityFramework uses it as key
        public string ConnectionId { get; set; }
        public string Username { get; set; }

    }
}