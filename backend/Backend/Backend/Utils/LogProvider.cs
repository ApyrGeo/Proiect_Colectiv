using log4net;

namespace Backend.Utils;

public static class LogProvider
{
    public static ILog CreateLogger<T>()
    {
        return LogManager.GetLogger(typeof(T));
    }
}
