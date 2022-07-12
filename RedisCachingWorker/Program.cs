using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace RedisCaching
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    
                    var redisOptions = configuration.GetSection(RedisOptions.ConfigSection).Get<RedisOptions>();
                    
                    var redisConfiguration = new RedisConfiguration()
                    {
                        AbortOnConnectFail = true,
                        KeyPrefix = "",
                        AllowAdmin = true,
                        ConnectTimeout = 1000,
                        SyncTimeout = 1000,
                        Database = 0,
                        Ssl = false,
                        Hosts = new[]
                        {
                            new RedisHost { Host = redisOptions.Host, Port = redisOptions.Port },
                        },
                        ServerEnumerationStrategy = new ServerEnumerationStrategy()
                        {
                            Mode = ServerEnumerationStrategy.ModeOptions.All,
                            TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                            UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                        }
                    };
                    services.AddSingleton(redisOptions);
                    services.AddSingleton(redisConfiguration);
                    services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
                    services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
                    services.AddSingleton<IRedisDefaultCacheClient, RedisDefaultCacheClient>();
                    services.AddSingleton<ISerializer, NewtonsoftSerializer>();

                    services.AddHostedService<Worker>();
                });
    }
}