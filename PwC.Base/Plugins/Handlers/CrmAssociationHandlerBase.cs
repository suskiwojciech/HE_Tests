using Microsoft.Xrm.Sdk.Client;
using PwC.Base.Plugins.Common;
using PwC.Base.Plugins.Common.PluginContexts;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;

namespace PwC.Base.Plugins.Handlers
{
    public abstract class CrmAssociationHandlerBase<TContext> : CrmHandlerBase<TContext>, ICrmWorkHandler
        where TContext : OrganizationServiceContext
    {
        protected new EntityReferencePluginExecutionContext ExecutionData { get; private set; }

        public abstract void DoWork();

        public override void Initialize(IServiceProvider serviceProvider, ICrmRepositoriesFactory crmRepositoriesFactory, ICrmServicesFactory crmServicesFactory, HandlerCache cache)
        {
            base.Initialize(serviceProvider, crmRepositoriesFactory, crmServicesFactory, cache);
            this.ExecutionData = new EntityReferencePluginExecutionContext(base.ExecutionData.Context);
        }
    }
}
