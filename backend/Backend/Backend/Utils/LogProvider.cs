using log4net;

namespace Backend.Utils;

public static class LogProvider
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(LogProvider));

    public static ILog GetLogger()
    {
        return _log;
    }
}
