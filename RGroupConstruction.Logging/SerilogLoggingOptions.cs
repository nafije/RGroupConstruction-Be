namespace RGroupConstruction.Logging;

public class SerilogLoggingOptions
{
    public string ApplicationName { get; set; } = "RGroupConstruction";
    public string MinimumLevel { get; set; } = "Information";
    public string MicrosoftLevel { get; set; } = "Warning";
    public string SystemLevel { get; set; } = "Warning";
    public string ConnectionStringName { get; set; } = "Default";
    public string TableName { get; set; } = "Logs";
    public bool StoreTimestampInUtc { get; set; } = true;
    public string OutputTemplate { get; set; } =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
}

