using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.Plugins.Common;
using PwC.Base.Plugins.Common.PluginContexts;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;

namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Base Crm Handler abstract class for delete handlers. It contains delete execution context dedicated for delete handler plugins.
    /// </summary>
    /// <typeparam name="TEntity">The type of the Crm entity.</typeparam>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Handlers.CrmHandlerBase{TContext}" />
    public abstract class CrmDeleteHandlerBase<TEntity, TContext> : CrmHandlerBase<TContext>, ICrmWorkHandler
        where TEntity : Entity, new()
        where TContext : OrganizationServiceContext
    {
        /// <summary>
        /// Gets plugin execution data for and entity
        /// </summary>
        protected new DeletePluginExecutionContext<TEntity> ExecutionData { get; private set; }

        /// <summary>
        /// Do Work method should implement logic that should invoke certain scenario for handler.
        /// In this method logic should access crm data base on Crm services and repositories. Those are accessable by
        /// CrmRepositoriesFactory and CrmServicesFactory.
        /// ExecutionData contains plugin context data to process.
        /// </summary>
        public abstract void DoWork();

        /// <summary>
        /// Initialization of Handler required data. this method must be invoked just after creation before using it's methods.
        /// </summary>
        /// <param name="serviceProvider">Plugin Service provider object.</param>
        /// <param name="crmRepositoriesFactory">Crm Repositories factory object for repositories creation.</param>
        /// <param name="crmServicesFactory">Crm Services factory object for services creation.</param>
        /// <param name="cache">Hadnlers cache which can distribute cached data in execution pipeline.</param>
        public override void Initialize(IServiceProvider serviceProvider, ICrmRepositoriesFactory crmRepositoriesFactory, ICrmServicesFactory crmServicesFactory, HandlerCache cache)
        {
            base.Initialize(serviceProvider, crmRepositoriesFactory, crmServicesFactory, cache);
            this.ExecutionData = new DeletePluginExecutionContext<TEntity>(base.ExecutionData.Context);
        }
    }
}