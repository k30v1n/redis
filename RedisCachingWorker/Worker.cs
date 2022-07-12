using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace RedisCaching
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRedisCacheClient _cache;

        public Worker(ILogger<Worker> logger, IRedisCacheClient cache, RedisOptions options)
        {
            _logger = logger;
            _cache = cache;

            _logger.LogInformation($"Connecting to host {options.Host}, port {options.Port}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    var keys = await _cache.Db0.SearchKeysAsync("*");
                    _logger.LogInformation("Current keys: {keys}", String.Join(',', keys));

                    var keysToDelete = await _cache.Db0.SearchKeysAsync("user_1_account_*");
                    if (keysToDelete.Count() > 0)
                    {
                        _logger.LogWarning(
                            $"Removing {keysToDelete.Count()} keys starting with `user_1_account_*` from DB 0");

                        await _cache.Db0.RemoveAllAsync(keysToDelete);
                    }

                    keys = await _cache.Db0.SearchKeysAsync("*");
                    _logger.LogInformation("Current keys: {keys}", String.Join(',', keys));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}