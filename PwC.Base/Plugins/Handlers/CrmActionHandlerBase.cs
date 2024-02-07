using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.Plugins.Common.Helpers;

namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Base Crm Handler abstract class for Action handlers.
    /// </summary>
    /// <typeparam name="TRequest">The type of the Organization request for which handler is developed.</typeparam>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Handlers.CrmHandlerBase{TContext}" />
    public abstract class CrmActionHandlerBase<TRequest, TContext> : CrmHandlerBase<TContext>, ICrmWorkHandler
        where TRequest : OrganizationRequest
        where TContext : OrganizationServiceContext
    {
        /// <summary>
        /// Name of the Crm Action for the request
        /// </summary>
        protected readonly string ActionName = NameHelper.GetActionNameFromRequest(typeof(TRequest));

        /// <summary>
        /// Do Work method should implement logic that should invoke certain scenario for handler.
        /// In this method logic should access crm data base on Crm services and repositories. Those are accessable by
        /// CrmRepositoriesFactory and CrmServicesFactory.
        /// ExecutionData contains plugin context data to process.
        /// </summary>
        public abstract void DoWork();
    }
}
