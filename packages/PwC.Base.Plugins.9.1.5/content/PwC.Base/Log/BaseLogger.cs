using Microsoft.Xrm.Sdk;

namespace PwC.Base.Log
{
    public class BaseLogger : IBaseLogger
    {
        public LogLevel LogLevel { get; private set; }

        private readonly ITracingService tracingService;

        public BaseLogger(ITracingService tracingService, LogSettings settings)
        {
            this.tracingService = tracingService;
            this.LogLevel = settings.Level;
        }

        public void Debug(string msg)
        {
            InternalLog(LogLevel.Debug, msg);
        }

        public void Error(string msg)
        {
            InternalLog(LogLevel.Error, msg);
        }

        public void Fatal(string msg)
        {
            InternalLog(LogLevel.Fatal, msg);
        }

        public void Info(string msg)
        {
            InternalLog(LogLevel.Info, msg);
        }

        public void Trace(string msg)
        {
            InternalLog(LogLevel.Trace, msg);
        }

        public void Warn(string msg)
        {
            InternalLog(LogLevel.Warn, msg);
        }

        private void InternalLog(LogLevel logLevel, string msg)
        {
            if (LogLevel <= logLevel)
            {
                tracingService.Trace($"[{logLevel.ToString().ToLower()}]: {msg}");
            }
        }
    }
}
