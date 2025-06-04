using BaseBotService.Core.Enums;
using BaseBotService.Core.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BaseBotService.Infrastructure.Services;
public class HealthCheckService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IEnvironmentService _environment;

    public HealthCheckService()
    {
        // Not using the regular DI here, as of using the AddHostedService of the IServiceCollection in program.cs
        // Maybe this can be refactored to use our existing instance of IServiceProvider?
        _logger = Program.ServiceProvider.GetRequiredService<ILogger>();
        _client = Program.ServiceProvider.GetRequiredService<DiscordSocketClient>();
        _environment = Program.ServiceProvider.GetRequiredService<IEnvironmentService>();
    }

    public Task<HealthCheckResult> CheckHealthAsync()
    {
        _logger.Debug($"Checking health of Discord client: {_client.ConnectionState}");
        return _client.ConnectionState switch
        {
            ConnectionState.Connected => Task.FromResult(HealthCheckResult.Healthy),
            ConnectionState.Connecting => Task.FromResult(HealthCheckResult.Degraded),
            _ => Task.FromResult(HealthCheckResult.Unhealthy),
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new(IPAddress.Any, _environment.HealthPort);
        listener.Start();
        _logger.Information($"Listening for health-probe on port ::{_environment.HealthPort}.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.Debug("Client connected");

                using NetworkStream stream = client.GetStream();
                string response = await CheckHealthAsync() switch
                {
                    HealthCheckResult.Healthy => "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nConnected",
                    HealthCheckResult.Degraded => "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nConnecting",
                    _ => "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\nDisconnected",
                };
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, stoppingToken);
                _logger.Debug("Response sent");
            }
        }
        finally
        {
            listener.Stop();
            _logger.Information($"Stopped listener for health-probe on port ::{_environment.HealthPort}");
        }
    }
}
