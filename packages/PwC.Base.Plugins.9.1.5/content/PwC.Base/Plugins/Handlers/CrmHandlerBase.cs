using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.Plugins.Common;
using PwC.Base.Plugins.Common.Constants;
using PwC.Base.Plugins.Common.PluginContexts;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;

namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Base Crm Handler abstract class which implements basic Crm Handler interface methods
    /// and adds some helper methods for handlers development.
    /// </summary>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Handlers.ICrmHandler" />
    public abstract class CrmHandlerBase<TContext> : ICrmHandler
        where TContext : OrganizationServiceContext
    {
        protected CrmHandlerBase()
        {
        }

        protected ITracingService TracingService { get; private set; }

        protected PluginExecutionContext ExecutionData { get; private set; }

        protected ICrmRepositoriesFactory CrmRepositoriesFactory { get; private set; }

        protected ICrmServicesFactory CrmServicesFactory { get; private set; }

        protected HandlerCache Cache { get; private set; }

        /// <summary>
        /// Initialization of Handler required data. this method must be invoked just after creation before using it's methods.
        /// </summary>
        /// <param name="serviceProvider">Plugin Service provider object.</param>
        /// <param name="crmRepositoriesFactory">Crm Repositories factory object for repositories creation.</param>
        /// <param name="servicesFactory">Crm Services factory object for services creation.</param>
        /// <param name="cache">Hadnlers cache which can distribute cached data in execution pipeline.</param>
        public virtual void Initialize(IServiceProvider serviceProvider, ICrmRepositoriesFactory crmRepositoriesFactory, ICrmServicesFactory servicesFactory, HandlerCache cache)
        {
            this.CrmRepositoriesFactory = crmRepositoriesFactory;
            this.CrmServicesFactory = servicesFactory;
            this.Cache = cache;
            this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            this.ExecutionData = new PluginExecutionContext(context);
        }

        /// <summary>
        /// Can work method determines if Handler should start invoking DoWork/DoValidation method.
        /// </summary>
        /// <returns>
        /// Return value should be true or false depending if handler should invoke.
        /// </returns>
        public abstract bool CanWork();

        /// <summary>
        /// Compares specified message with plugin execution context message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>True if equal</returns>
        protected bool IsMessage(CrmMessage message)
        {
            return ExecutionData.IsMessage(message);
        }

        /// <summary>
        /// Compares specified message with plugin execution context message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>True if equal</returns>
        protected bool IsMessage(string message)
        {
            return ExecutionData.IsMessage(message);
        }

        /// <summary>
        /// Compares specified stage with plugin execution context stage
        /// </summary>
        /// <param name="stage">Step</param>
        /// <returns>True if equal</returns>
        protected bool IsStage(CrmProcessingStepStages stage)
        {
            return ExecutionData.Context.Stage == (int)stage;
        }

        /// <summary>
        /// Gets and initializes base crm entity repository.
        /// </summary>
        /// <typeparam name="TEntity">Type of the crm entity fow which repository should be generated.</typeparam>
        /// <returns>Crm entity base repository object.</returns>
        protected ICrmEntityRepository<TEntity, TContext> GetBaseRepository<TEntity>()
            where TEntity : Entity, new()
        {
            return CrmRepositoriesFactory.GetBase<TEntity, TContext>();
        }
    }
}