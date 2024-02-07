using System.Runtime.Serialization;

namespace PwC.Base.Log
{
    [DataContract]
    public class LogSettings
    {
        [DataMember]
        public LogLevel Level { get; set; }
    }
}