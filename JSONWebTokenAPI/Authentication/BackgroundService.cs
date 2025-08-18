using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog.Core;

namespace JSONWebTokenAPI.Authentication
{
    public class BackgroundServiceTask : BackgroundService
    {
        private readonly ILogger<BackgroundServiceTask> logger;
        public BackgroundServiceTask(ILogger<BackgroundServiceTask> _logger)
        {
            logger = _logger;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("background service started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10),cancellationToken);
            }
            logger.LogInformation("background service ended");
        }
    }
}
