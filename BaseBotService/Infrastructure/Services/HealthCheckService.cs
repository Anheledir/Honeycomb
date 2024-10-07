using BaseBotService.Core.Enums;
using BaseBotService.Core.Interfaces;
using Discord.WebSocket;
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
    private TcpListener? _listener;

    public HealthCheckService(ILogger logger, DiscordSocketClient client, IEnvironmentService environment)
    {
        _logger = logger.ForContext<HealthCheckService>();
        _client = client;
        _environment = environment;
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
        try
        {
            _listener = new TcpListener(IPAddress.Any, _environment.HealthPort);
            _listener.Start();
            _logger.Information($"Listening for health-probe on port ::{_environment.HealthPort}.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using TcpClient client = await _listener.AcceptTcpClientAsync(stoppingToken);
                    _logger.Debug("Client connected");

                    using NetworkStream stream = client.GetStream();
                    string response = await GenerateHealthResponseAsync(stoppingToken);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes, stoppingToken);
                    _logger.Debug("Response sent");
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.Error(ex, "Error handling health-probe request");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Critical failure in health check service");
        }
        finally
        {
            _listener?.Stop();
            _logger.Information($"Stopped listener for health-probe on port ::{_environment.HealthPort}");
        }
    }

    private async Task<string> GenerateHealthResponseAsync(CancellationToken stoppingToken)
    {
        HealthCheckResult result = await CheckHealthAsync();
        return result switch
        {
            HealthCheckResult.Healthy => "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nConnected",
            HealthCheckResult.Degraded => "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nConnecting",
            _ => "HTTP/1.1 500 Internal Server Error\r\nContent-Type: text/plain\r\n\r\nDisconnected",
        };
    }

    public override void Dispose()
    {
        _listener?.Stop();
        base.Dispose();
    }
}
