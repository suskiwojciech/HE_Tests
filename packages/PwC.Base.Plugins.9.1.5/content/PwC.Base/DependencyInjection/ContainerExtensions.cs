using System;

namespace PwC.Base.DependencyInjection
{
    /// <summary>
    /// Container helper methods
    /// </summary>
    public static class ContainerExtensions
    {
        public static T Resolve<T>(this IContainer container, params IConstructorParameter[] parameters)
        {
            Type serviceType = typeof(T);
            return (T)container.Resolve(serviceType, parameters);
        }

        public static void RegisterSingleton<TService>(this IContainer container, TService instance)
        {
            container.Register(() => instance);
        }

        public static void RegisterSingleton<TService>(this IContainer container, Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            container.Register(() => lazy.Value);
        }
    }
}
