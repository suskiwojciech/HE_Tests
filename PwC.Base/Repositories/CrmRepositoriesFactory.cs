using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.DependencyInjection;
using System;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// Implementation of CrmRepositoriesFacctory interface
    /// </summary>
    /// <seealso cref="PwC.Base.Repositories.ICrmRepositoriesFactory" />
    public class CrmRepositoriesFactory : ICrmRepositoriesFactory
    {
        private readonly IContainer container;
        private readonly IOrganizationServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmRepositoriesFactory"/> class.
        /// </summary>
        /// <param name="container">DI container instance.</param>
        public CrmRepositoriesFactory(IContainer container)
        {
            this.serviceFactory = container.Resolve<IOrganizationServiceFactory>();
            this.container = container;
        }

        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to current user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <returns>
        /// Initialized repository object
        /// </returns>
        public virtual TRepository Get<TRepository>()
            where TRepository : ICrmRepository
        {
            return this.Get<TRepository>(Guid.Empty);
        }

        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to specified user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <param name="callerId">Identifier of (user) caller for the crm connection.</param>
        /// <returns>
        /// Initialized repository object
        /// </returns>
        public virtual TRepository Get<TRepository>(Guid callerId)
            where TRepository : ICrmRepository
        {
            return container.Resolve<TRepository>(CreateCrmRepositoryArgsParameter(callerId));
        }

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to specified user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        public virtual ICrmEntityRepository<TEntity, TContext> GetBase<TEntity, TContext>()
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext
        {
            return GetBase<TEntity, TContext>(Guid.Empty);
        }

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to specified user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <param name="callerId">Identifier of (user) caller for the crm connection.</param>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        public virtual ICrmEntityRepository<TEntity, TContext> GetBase<TEntity, TContext>(Guid callerId)
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext
        {
            return new CrmEntityRepository<TEntity, TContext>(CreateCrmRepositoryArgs(callerId));
        }

        /// <summary>
        /// Gets specified repository and initialize it. Crm organization service is generated in relation to the SYSTEM user.
        /// </summary>
        /// <typeparam name="TRepository">Type of custom CrmRepository.</typeparam>
        /// <returns>
        /// Initialized repository object
        /// </returns>
        public TRepository GetSystem<TRepository>()
            where TRepository : ICrmRepository
        {
            return container.Resolve<TRepository>(CreateCrmRepositoryArgsParameter(null));
        }

        /// <summary>
        /// Gets specified base (generic) entity repository and initialize it.
        /// Crm organization service is generated in relation to SYSTEM user.
        /// </summary>
        /// <typeparam name="TEntity">The type of the Crm entity in relation to which generic repository will be generated.</typeparam>
        /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
        /// <returns>
        /// Initialized generic entity repository object
        /// </returns>
        public ICrmEntityRepository<TEntity, TContext> GetSystemBase<TEntity, TContext>()
            where TEntity : Entity, new()
            where TContext : OrganizationServiceContext
        {
            return new CrmEntityRepository<TEntity, TContext>(CreateCrmRepositoryArgs(null));
        }

        private TypedConstructorParameter CreateCrmRepositoryArgsParameter(Guid? userId)
        {
            var args = CreateCrmRepositoryArgs(userId);
            return new TypedConstructorParameter(typeof(CrmRepositoryArgs), args);
        }

        private CrmRepositoryArgs CreateCrmRepositoryArgs(Guid? userId)
        {
            var service = this.serviceFactory.CreateOrganizationService(userId);
            var args = container.Resolve<CrmRepositoryArgs>(new TypedConstructorParameter(typeof(IOrganizationService), service));
            return args;
        }
    }
}