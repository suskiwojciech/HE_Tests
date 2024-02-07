using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PwC.Base.Log
{
    public class TraceLogContext : IDisposable
    {
        private readonly IBaseLogger logger;
        private readonly string callContext = "";
        private readonly Stopwatch stopwatch;

        protected TraceLogContext(IBaseLogger logger, string callContext = "", IList<TraceParameterData> parameters = null)
        {
            this.logger = logger;
            this.callContext = callContext;
            this.stopwatch = new Stopwatch();

            this.stopwatch.Start();
            logger.Debug($"{callContext} called");

            if (this.logger.LogLevel == LogLevel.Trace && parameters != null && parameters.Any())
            {
                this.logger.Trace("With input:");
                foreach (var param in parameters)
                {
                    this.logger.Trace($"    {param}");
                }
            }
        }

        public TraceLogContext()
        {
            // No logs
        }

        public void Dispose()
        {
            if (stopwatch != null)
            {
                this.stopwatch.Stop();
                logger.Debug($"{callContext} execution finished. Elapsed timed: {this.stopwatch.Elapsed.TotalMilliseconds}ms");
            }
        }

        public static TraceLogContext Create(IBaseLogger logger, params object[] parameters)
        {
            if (logger.LogLevel <= LogLevel.Debug)
            {
                StackFrame frame = new StackFrame(2);
                var method = frame.GetMethod();
                var type = method?.DeclaringType;
                var name = method?.Name;

                StackFrame parentFrame = new StackFrame(3);
                var parentMethod = parentFrame.GetMethod();
                var parentType = parentMethod?.DeclaringType;
                var parentName = parentMethod?.Name;

                string callContext = $"{parentType}.{parentName} -> {type}.{name}";

                if (logger.LogLevel == LogLevel.Trace)
                {
                    var parametersData = parameters?.Select((p, i) => new TraceParameterData($"Param{i}", SerializeInput(p))).ToList();
                    return new TraceLogContext(logger, callContext, parametersData);
                }
                else
                {
                    return new TraceLogContext(logger, callContext);
                }
            }
            else
            {
                return new TraceLogContext();
            }
        }

        private static string SerializeInput(object param)
        {
            string json = string.Empty;

            try
            {
                if (param != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(param.GetType());
                        serializer.WriteObject(memoryStream, param);
                        json = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
                else
                {
                    json = "Null";
                }
            }
            catch
            {
                try
                {
                    json = (param ?? "Null").ToString();
                }
                catch
                {
                    json = "[Could not serialize parameter]";
                }
            }

            return json;
        }
    }
}