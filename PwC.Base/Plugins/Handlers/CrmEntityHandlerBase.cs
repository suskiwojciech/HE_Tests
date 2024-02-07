using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.Plugins.Common;
using PwC.Base.Plugins.Common.Helpers;
using PwC.Base.Plugins.Common.PluginContexts;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;
using System.Linq.Expressions;

namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Base Crm Handler abstract class for Entity handlers. It contains Entity execution context dedicated for entity handler plugins.
    /// </summary>
    /// <typeparam name="TEntity">The type of the Crm entity.</typeparam>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Handlers.CrmHandlerBase{TContext}" />
    public abstract class CrmEntityHandlerBase<TEntity, TContext> : CrmHandlerBase<TContext>, ICrmWorkHandler
        where TEntity : Entity, new()
        where TContext : OrganizationServiceContext
    {
        private TEntity currentState;

        /// <summary>
        /// Gets current entity state combining fields values from both target and preimage values.
        /// If postImage exist it will return it.
        /// </summary>
        internal TEntity CurrentState
        {
            get
            {
                if (currentState == null)
                {
                    if (ExecutionData.PostImageExists())
                    {
                        currentState = ExecutionData.PostImage;
                    }
                    else if (ExecutionData.PreImageExists())
                    {
                        currentState = ExecutionData.GetCurrentState();
                    }
                    else
                    {
                        currentState = ExecutionData.Target;
                    }
                }

                return currentState;
            }
        }

        /// <summary>
        /// Gets plugin execution data for and entity
        /// </summary>
        protected new EntityPluginExecutionContext<TEntity> ExecutionData { get; private set; }

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
            this.ExecutionData = new EntityPluginExecutionContext<TEntity>(base.ExecutionData.Context);
        }

        /// <summary>
        /// Checks if specified field changed
        /// </summary>
        /// <param name="field">Field name</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged(string field)
        {
            return ExecutionData.ValueChanged(field);
        }

        /// <summary>
        /// Checks if specified field changed
        /// </summary>
        /// <param name="selector">Selected property</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged<T>(Expression<Func<TEntity, T>> selector)
        {
            string fieldName = NameHelper.ExtractEntityAttributeName(selector);
            return ExecutionData.ValueChanged(fieldName);
        }

        /// <summary>
        /// Checks if specified field changed to the specified value
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="newValue">Expected value</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged(string field, object newValue)
        {
            return ExecutionData.ValueChanged(field, newValue);
        }

        /// <summary>
        /// Checks if specified field changed to the specified value
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="selector">Selected property</param>
        /// <param name="newValue">Expected value</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged<T>(Expression<Func<TEntity, T>> selector, T newValue)
        {
            string fieldName = NameHelper.ExtractEntityAttributeName(selector);
            return ExecutionData.ValueChanged(fieldName, newValue);
        }

        /// <summary>
        /// Checks if specified field changed from oldValue to the newValue
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="oldValue">Old valuie</param>
        /// <param name="newValue">New value</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged(string field, object oldValue, object newValue)
        {
            return ExecutionData.ValueChanged(field, oldValue, newValue);
        }

        /// <summary>
        /// Checks if specified field changed from oldValue to the newValue
        /// </summary>
        /// <param name="selector">Selected property</param>
        /// <param name="oldValue">Old valuie</param>
        /// <param name="newValue">New value</param>
        /// <returns>True if field was changed</returns>
        protected bool ValueChanged<T>(Expression<Func<TEntity, T>> selector, object oldValue, object newValue)
        {
            string fieldName = NameHelper.ExtractEntityAttributeName(selector);
            return ExecutionData.ValueChanged(fieldName, oldValue, newValue);
        }
    }
}