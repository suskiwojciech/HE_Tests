using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins.Common.Constants;
using System;
using System.Linq;

namespace PwC.Base.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CrmMessageAttribute : Attribute
    {
        // For CLS-compliant
        public CrmMessageAttribute(CrmMessage message) :
            this(new CrmMessage[] {  message })
        {

        }

        public CrmMessageAttribute(params CrmMessage[] messages)
        {
            Messages = messages;
        }

        public CrmMessage[] Messages { get; set; }

        public bool IsFilterFulfilled(Type handlerType, IPluginExecutionContext context)
        {
            if (Enum.TryParse(context.MessageName, out CrmMessage messageType))
            {
                return Messages.Contains(messageType);
            }
            else
            {
                return false;
            }
        }
    }
}
