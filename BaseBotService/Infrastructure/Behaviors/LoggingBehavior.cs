namespace BaseBotService.Infrastructure.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger logger)
    {
        _logger = logger.ForContext<LoggingBehavior<TRequest, TResponse>>();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {RequestName} {@Request}", typeof(TRequest).Name, request);

        var response = await next();

        _logger.Information("Handled {RequestName} {@Response}", typeof(TRequest).Name, response);

        return response;
    }
}