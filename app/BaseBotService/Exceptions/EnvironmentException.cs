using BaseBotService.Enumeration;
using System.Runtime.Serialization;

namespace BaseBotService.Exceptions;

[Serializable]
public class EnvironmentException : Exception
{
    public EnvironmentSetting Setting { get; set; }

    public EnvironmentException()
    {
        Setting = EnvironmentSetting.Unknown;
    }

    public EnvironmentException(EnvironmentSetting setting)
    {
        Setting = setting;
    }

    public EnvironmentException(EnvironmentSetting setting, string? message) : base(message)
    {
        Setting = setting;
    }

    public EnvironmentException(EnvironmentSetting setting, string? message, Exception? innerException) : base(message, innerException)
    {
        Setting = setting;
    }

    public EnvironmentException(string? message) : this(EnvironmentSetting.Unknown, message)
    {
    }

    public EnvironmentException(string? message, Exception? innerException) : this(EnvironmentSetting.Unknown, message, innerException)
    {
    }

    protected EnvironmentException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
