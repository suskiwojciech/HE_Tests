using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// Crm Repositories factory interface describes functions to create and initialize Crm Repositories.
    /// Factory track and monitor repositories to initailize only a single instance of each one.
    /// </summary>
    public interface ICrmRepositoriesFactory
    {
        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to specified user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <param name="callerId">Identifier of (user) caller for the crm connection.</param>
        /// <returns>Initialized repository object</returns>
        TRepository Get<TRepository>(Guid callerId)
            where TRepository : ICrmRepository;

        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to current user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <returns>
        /// Initialized repository object
        /// </returns>
        TRepository Get<TRepository>()
            where TRepository : ICrmRepository;

        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to the SYSTEM user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <returns>
        /// Initialized repository object
        /// </returns>
        TRepository GetSystem<TRepository>()
            where TRepository : ICrmRepository;

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to specified user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <param name="callerId">Identifier of (user) caller for the crm connection. Null for SYSTEM</param>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        ICrmEntityRepository<TEntity, TContext> GetBase<TEntity, TContext>(Guid callerId)
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext;

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to current user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        ICrmEntityRepository<TEntity, TContext> GetBase<TEntity, TContext>()
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext;

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to SYSTEM user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        ICrmEntityRepository<TEntity, TContext> GetSystemBase<TEntity, TContext>()
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext;
    }
}
