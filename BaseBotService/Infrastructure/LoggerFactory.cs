using BaseBotService.Utilities.Extensions;
using Serilog.Events;

namespace BaseBotService.Infrastructure;

/// <summary>
/// Provides functionality to create and configure a Serilog logger with customized settings, including log level
/// and sensitive data masking.
/// </summary>
/// <example>
/// The following example demonstrates how to use the <see cref="LoggerFactory"/> to log sensitive and regular data.
/// 
/// <code>
/// // Create the logger using the LoggerFactory
/// ILogger logger = LoggerFactory.CreateLogger();
/// 
/// // Example 1: Logging regular data
/// logger.Information("User {Username} has logged in.", "JohnDoe");
/// 
/// // Example 2: Logging sensitive data
/// string token = "mysecrettoken123";
/// logger.Information("User {Username} with token {Token} has logged in.", "JohnDoe", token);
/// 
/// // The token will be automatically masked in the logs as per the MaskToken extension method
/// // resulting in a log output similar to:
/// // User JohnDoe with token *********************ken123 has logged in.
/// </code>
/// </example>
public static class LoggerFactory
{
    private const string DefaultLogLevel = "Information";
    private const string LogLevelEnvVariable = "LOGLEVEL";

    /// <summary>
    /// Creates and configures a Serilog logger instance, including settings for log level and data masking.
    /// </summary>
    /// <returns>A configured <see cref="ILogger"/> instance.</returns>
    public static ILogger CreateLogger()
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        // Determine the log level from the environment variable
        LogEventLevel logLevel = GetLogLevelFromEnvironment();
        loggerConfig.MinimumLevel.Is(logLevel);

        // Mask sensitive data in logs
        loggerConfig.Destructure.ByTransforming<LogEventPropertyValue>(value =>
        {
            if (value is ScalarValue scalarValue && scalarValue.Value is string strValue)
            {
                return new ScalarValue(strValue.MaskToken());
            }
            return value;
        });

        Log.Logger = loggerConfig.CreateLogger();
        return Log.Logger;
    }

    /// <summary>
    /// Determines the log level based on an environment variable.
    /// </summary>
    /// <returns>The <see cref="LogEventLevel"/> to be used for logging.</returns>
    private static LogEventLevel GetLogLevelFromEnvironment()
    {
        string? logLevelString = Environment.GetEnvironmentVariable(LogLevelEnvVariable);
        if (Enum.TryParse(logLevelString, true, out LogEventLevel logLevel))
        {
            return logLevel;
        }

        // Log that the environment variable was not found or invalid and defaulting to Information
        Log.Logger.Information($"Environment variable {LogLevelEnvVariable} is not set or invalid. Defaulting to {DefaultLogLevel}.");
        return Enum.TryParse(DefaultLogLevel, true, out LogEventLevel defaultLevel) ? defaultLevel : LogEventLevel.Information;
    }
}
