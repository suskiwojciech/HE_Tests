using PwC.Base.Log;
using System.Runtime.Serialization;

namespace PwC.Base.Plugins
{
    [DataContract]
    public class PluginSettings
    {
        [DataMember]
        public LogSettings LogSettings { get; set; }
    }
}