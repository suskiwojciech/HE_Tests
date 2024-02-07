using PwC.Base.Plugins.Common;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;

namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Crm Handler interface describes how basic plugin handler should look like.
    /// </summary>
    public interface ICrmHandler
    {
        /// <summary>
        /// Can work method determines if Handler should start invoking relevant method.
        /// </summary>
        /// <returns>Return value should be true or false depending if handler should invoke.</returns>
        bool CanWork();

        /// <summary>
        /// Initialization of Handler required data. this method must be invoked just after creation before using it's methods.
        /// </summary>
        /// <param name="serviceProvider">Plugin Service provider object.</param>
        /// <param name="repositoryFactory">Crm Repositories factory object for repositories creation.</param>
        /// <param name="servicesFactory">Crm Services factory object for services creation.</param>
        /// <param name="cache">Hadnlers cache which can distribute cached data in execution pipeline.</param>
        void Initialize(IServiceProvider serviceProvider, ICrmRepositoriesFactory repositoryFactory, ICrmServicesFactory servicesFactory, HandlerCache cache);
    }
}
