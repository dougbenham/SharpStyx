using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace SharpStyx;

public class Monitor : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Monitor(IHostApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var process in Process.GetProcessesByName("D2R"))
            {
                var context = new ProcessContext(process);
                var gameIPOffset = context.GetGameIPOffset();
            }

            await Task.Delay(5000, stoppingToken);
        }

        _applicationLifetime.StopApplication();
    }
}

public class UI : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;

    public UI(IHostApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            stoppingToken.Register(() => Application.Exit());

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            _applicationLifetime.StopApplication();
        }, stoppingToken);
    }
}