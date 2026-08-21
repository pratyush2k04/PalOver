using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pdr.Api.Constants;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class BlockerEscalationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BlockerEscalationBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(BlockerEscalationConstants.BackgroundService.CheckIntervalHours);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(BlockerEscalationConstants.BackgroundService.ServiceStarted);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var escalationService = scope.ServiceProvider.GetRequiredService<IBlockerEscalationService>();
            int count = await escalationService.ProcessEscalationsAsync();

            if (count > 0)
            {
                logger.LogInformation(BlockerEscalationConstants.BackgroundService.ProcessedEscalations, count);
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }
}
