using PwC.Base.DependencyInjection;

namespace PwC.Base.Services
{
    /// <summary>
    /// Implementation of basic CrmService factory interface
    /// </summary>
    /// <seealso cref="PwC.Base.Services.ICrmServicesFactory" />
    public class CrmServicesFactory : ICrmServicesFactory
    {
        private readonly IContainer container;

        /// <summary>
        ///  Initializes a new instance of the <see cref="CrmServicesFactory"/> class.
        /// </summary>
        /// <param name="container">DI container instance.</param>
        public CrmServicesFactory(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Gets initialized instance of the CrmService
        /// </summary>
        /// <typeparam name="TService">Type of CrmService.</typeparam>
        /// <returns>
        /// Initialized object of given CrmService type.
        /// </returns>
        public virtual TService Get<TService>()
            where TService : ICrmService
        {
            return container.Resolve<TService>();
        }
    }
}