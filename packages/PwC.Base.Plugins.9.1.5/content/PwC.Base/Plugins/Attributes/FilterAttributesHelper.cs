using Microsoft.Xrm.Sdk;
using System;

namespace PwC.Base.Plugins.Attributes
{
    public static class FilterAttributesHelper
    {
        /// <summary>
        /// Checks if handler attribute filters are fulfilled
        /// </summary>
        /// <param name="handlerType">Handler type</param>
        /// <param name="context">IPluginExecutionContext handler context</param>
        /// <returns>True if all filters fullfiled</returns>
        public static bool AreFiltersFulfilled(Type handlerType, IPluginExecutionContext context)
        {
            var messageFilter = Attribute.GetCustomAttribute(handlerType, typeof(CrmMessageAttribute), inherit: true) as CrmMessageAttribute;
            var stepFilter = Attribute.GetCustomAttribute(handlerType, typeof(CrmProcessingStageAttribute), inherit: true) as CrmProcessingStageAttribute;

            Entity entityTarget = null;
            if (context.InputParameters.Contains("Target"))
            {
                entityTarget = context.InputParameters["Target"] as Entity;

                bool messageFilterFulfilled = messageFilter == null || messageFilter.IsFilterFulfilled(handlerType, context);
                if (!messageFilterFulfilled)
                {
                    return false;
                }

                bool stepFilterFulfilled = stepFilter == null || stepFilter.IsFilterFulfilled(handlerType, context);
                if (!stepFilterFulfilled)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
