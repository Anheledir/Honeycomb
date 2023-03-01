using BaseBotService.Enumeration;
using BaseBotService.Interfaces;
using Discord.WebSocket;
using Honeycomb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BaseBotService.Helpers;
public class HealthCheckService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IEnvironmentService _environment;

    public HealthCheckService()
    {
        _logger = Program.ServiceProvider.GetRequiredService<ILogger>();
        _client = Program.ServiceProvider.GetRequiredService<DiscordSocketClient>();
        _environment = Program.ServiceProvider.GetRequiredService<IEnvironmentService>();
    }

    public Task<HealthCheckResult> CheckHealthAsync()
    {
        _logger.Information($"Checking health of Discord client: {_client.ConnectionState}");
        switch (_client.ConnectionState)
        {
            case Discord.ConnectionState.Connected:
                return Task.FromResult(HealthCheckResult.Healthy);
            case Discord.ConnectionState.Connecting:
                return Task.FromResult(HealthCheckResult.Degraded);
            default:
                return Task.FromResult(HealthCheckResult.Unhealthy);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, _environment.HealthPort);
        listener.Start();
        _logger.Information($"Listening for health-probe on port ::{_environment.HealthPort}.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var client = await listener.AcceptTcpClientAsync();
            _logger.Debug("Client connected");

            using (NetworkStream stream = client.GetStream())
            {
                string response;
                switch (await CheckHealthAsync())
                {
                    case HealthCheckResult.Healthy:
                        response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nOK";
                        break;
                    default:
                        response = "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\nERROR";
                        break;
                }
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, stoppingToken);
                _logger.Debug("Response sent");
            }
        }
        _logger.Information($"Stopped listener for health-probe on port ::{_environment.HealthPort}");
    }
}
