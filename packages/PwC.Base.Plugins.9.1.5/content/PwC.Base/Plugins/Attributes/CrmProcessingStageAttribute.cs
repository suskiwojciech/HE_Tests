using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins.Common.Constants;
using System;
using System.Linq;

namespace PwC.Base.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CrmProcessingStageAttribute : Attribute
    {
        // For CLS-compliant
        public CrmProcessingStageAttribute(CrmProcessingStepStages stage) :
            this(new CrmProcessingStepStages[] { stage })
        {

        }

        public CrmProcessingStageAttribute(params CrmProcessingStepStages[] stages)
        {
            Stages = stages;
        }

        public CrmProcessingStepStages[] Stages { get; set; }

        public bool IsFilterFulfilled(Type handlerType, IPluginExecutionContext context)
        {
            if (Enum.IsDefined(typeof(CrmProcessingStepStages), context.Stage))
            {
                CrmProcessingStepStages stageType = (CrmProcessingStepStages)context.Stage;
                return Stages.Contains(stageType);
            }
            else
            {
                return false;
            }
        }
    }
}
