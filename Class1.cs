using Context;
using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins;
using PwC.Base.Plugins.Common.Constants;
using PwC.Base.Plugins.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL_Tests
{
    [CrmPluginRegistration(
       MessageNameEnum.Update,
       Account.EntityLogicalName,
       StageEnum.PreOperation,
       ExecutionModeEnum.Synchronous,
       "statuscode",
       "PL_Tests: Update",
       1,
       IsolationModeEnum.Sandbox,
       Id = "3FF1ED1C-DB8E-4D07-8966-232C74189FD5",
       Image1Name = "PreImage", Image1Attributes = "",
       Image1Type = ImageTypeEnum.PreImage)]
    public class Class1 : PluginBase<CrmContext>, IPlugin
    {
        public override void RegisterHandlers(CrmHandlerFactory<CrmContext> handlerFactory, IList<ICrmHandler> registeredHandlers)
        {
            throw new NotImplementedException();
        }
    }
}
