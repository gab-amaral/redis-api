namespace Redis.Domain.Configuration
{
    public class RedisConfiguration
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public int DefaultDatabase { get; set; }
        public string ConnectionString
        {
            get
            {
                return $"{HostName}:{Port},defaultDatabase={DefaultDatabase}";
            }
        }
    }
}
