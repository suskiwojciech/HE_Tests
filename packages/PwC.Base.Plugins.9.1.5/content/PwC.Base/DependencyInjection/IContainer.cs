using System;

namespace PwC.Base.DependencyInjection
{
    /// <summary>
    /// DI container used in CRM plugins. This should not be used outside CRM plugins project.
    /// For external project it is strongly recommended to implement adapter of this interface for the dedicated DI framework.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Register factory method for service type
        /// </summary>
        /// <typeparam name="TService">Type to register</typeparam>
        /// <param name="factoryMethod">Factory method implementation</param>
        void Register<TService>(Func<TService> factoryMethod);

        /// <summary>
        /// Register implementation type for service type
        /// </summary>
        /// <typeparam name="TService">Type to register</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        void Register<TService, TImplementation>()
            where TImplementation : TService;

        /// <summary>
        /// Resolve service type implementation
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="parameters">Optional parameters which will be passed to the constructor</param>
        /// <returns>Resolved implementation</returns>
        object Resolve(Type serviceType, params IConstructorParameter[] parameters);
    }
}
