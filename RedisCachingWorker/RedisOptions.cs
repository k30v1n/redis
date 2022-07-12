namespace RedisCaching
{
    public class RedisOptions
    {
        public const string ConfigSection = "Redis";
        
        public string Host { get; set; }
        public int Port { get; set; }
    }
}