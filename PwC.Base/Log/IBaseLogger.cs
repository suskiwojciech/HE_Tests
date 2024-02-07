namespace PwC.Base.Log
{
    public interface IBaseLogger
    {
        LogLevel LogLevel { get; }
        void Trace(string msg);

        void Debug(string msg);

        void Info(string msg);

        void Warn(string msg);

        void Error(string msg);

        void Fatal(string msg);
    }
}