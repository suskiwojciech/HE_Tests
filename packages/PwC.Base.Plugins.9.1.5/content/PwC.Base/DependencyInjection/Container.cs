using PwC.Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PwC.Base.DependencyInjection
{
    /// <summary>
    /// Default DI container. Should not be used outside CRM plugins library.
    /// </summary>
    public sealed class Container : IContainer
    {
        private readonly object registrationLock = new object();
        private readonly Dictionary<Type, Registration> registrations = new Dictionary<Type, Registration>();

        public void Register<TService>(Func<TService> factoryMethod)
        {
            lock (registrationLock)
            {
                registrations.Add(
                typeof(TService),
                new Registration(typeof(TService), () => factoryMethod()));
            }
        }

        public void Register<TService, TImplementation>()
            where TImplementation : TService
        {
            lock (registrationLock)
            {
                registrations.Add(
                    typeof(TService),
                    new Registration(typeof(TService), typeof(TImplementation)));
            }
        }

        public object Resolve(Type serviceType, params IConstructorParameter[] parameters)
        {
            if (this.registrations.TryGetValue(serviceType, out Registration registration))
            {
                if (registration.HasFactoryMethod)
                {
                    return registration.FactoryMethod();
                }

                if (registration.ImplementationType != null)
                {
                    return this.CreateInstance(registration.ImplementationType, parameters);
                }

                throw new ContainerException("Invalid type registration for " + serviceType);
            }
            else if (!serviceType.IsAbstract)
            {
                return this.CreateInstance(serviceType, parameters);
            }
            else if (serviceType.IsAbstract || serviceType.IsInterface)
            {
                var implementationType = ImplementationTypeCache.GetDefaultImplementationType(serviceType);
                return this.CreateInstance(implementationType, parameters);
            }
            else
            {
                throw new ContainerException("Could not resolve implementation for " + serviceType);
            }
        }

        private object CreateInstance(Type implementationType, params IConstructorParameter[] parameters)
        {
            // This is simple di implementation so only constructor is supported for now
            var ctors = implementationType.GetConstructors();

            if (ctors.Length > 1)
            {
                throw new ContainerException("Default container support classes with single constructor only");
            }

            var ctor = ctors.Single();
            var dependencies = ResolveConstructorDependencies(ctor, parameters);

            return Activator.CreateInstance(implementationType, dependencies);
        }

        private object[] ResolveConstructorDependencies(ConstructorInfo ctor, params IConstructorParameter[] parameters)
        {
            var ctorParameters = ctor.GetParameters();

            var dependencies = ctorParameters.Select(parameterInfo =>
            {
                if (parameters != null && parameters.Any())
                {
                    try
                    {
                        var configuredParameter = parameters.SingleOrDefault(constructorParameter => constructorParameter.IsValid(parameterInfo));

                        if (configuredParameter != null)
                        {
                            return configuredParameter.Instance;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ContainerException($"Could not resolve constructor parameter properly '{ctor.DeclaringType}' - '{parameterInfo.Name}'", ex);
                    }
                }
                return this.Resolve(parameterInfo.ParameterType);
            }).ToArray();

            return dependencies;
        }

        private static class ImplementationTypeCache
        {
            private static readonly object CacheLock = new object();
            private static Dictionary<Type, Type> cachedTypes = new Dictionary<Type, Type>();

            public static Type GetDefaultImplementationType(Type serviceType)
            {
                lock (CacheLock)
                {
                    if (!cachedTypes.ContainsKey(serviceType))
                    {
                        foreach (var type in AssemblyTypesCache.AllTypes)
                        {
                            if (serviceType.IsAssignableFrom(type) && type.IsClass)
                            {
                                try
                                {
                                    cachedTypes.Add(serviceType, type);
                                }
                                catch (Exception ex)
                                {
                                    throw new ContainerException($"Possible multiple implementations found for {serviceType}", ex);
                                }
                            }
                        }
                    }
                }

                try
                {
                    return cachedTypes[serviceType];
                }
                catch (Exception ex)
                {
                    throw new ContainerException("Could not resolve implementation for " + serviceType, ex);
                }
            }

            public static Type GetDefaultImplementationType<TService>()
            {
                return GetDefaultImplementationType(typeof(TService));
            }
        }

        /// <summary>
        /// Stores info about types registration
        /// </summary>
        private class Registration
        {
            public Registration(Type serviceType, Func<object> factoryMethod)
            {
                ServiceType = serviceType;
                FactoryMethod = factoryMethod;
            }

            public Registration(Type serviceType, Type implementationType)
            {
                ServiceType = serviceType;
                ImplementationType = implementationType;
            }

            public bool HasFactoryMethod
            {
                get
                {
                    return FactoryMethod != null;
                }
            }

            public Func<object> FactoryMethod { get; private set; }

            public Type ImplementationType { get; private set; }

            public Type ServiceType { get; private set; }
        }
    }
}
