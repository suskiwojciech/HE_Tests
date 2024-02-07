using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using PwC.Base.DependencyInjection;
using PwC.Base.Log;
using PwC.Base.Plugins.Attributes;
using PwC.Base.Plugins.Common;
using PwC.Base.Plugins.Handlers;
using PwC.Base.Repositories;
using PwC.Base.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;

namespace PwC.Base.Plugins
{
    /// <summary>
    /// Base Crm Plugin abstract class. Every declared plugin should be inherited from this class.
    /// Inside such generated plugin handlers should be registered in RegisterHandlers method.
    /// </summary>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="Microsoft.Xrm.Sdk.IPlugin" />
    public abstract class PluginBase<TContext> : IPlugin
        where TContext : OrganizationServiceContext
    {
        /// <summary>
        /// Plugin settings
        /// </summary>
        protected PluginSettings Settings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase{TContext}"/> class.
        /// This constructor enables inserting custom repositories, it enables mocking capabilities.
        /// </summary>
        /// <param name="crmRepositoriesFactory">The CRM repositories factory.</param>
        /// <param name="crmServicesFactory">The CRM services factory.</param>
        protected PluginBase(ICrmRepositoriesFactory crmRepositoriesFactory, ICrmServicesFactory crmServicesFactory)
        {
            this.CrmRepositoriesFactory = crmRepositoriesFactory;
            this.CrmServicesFactory = crmServicesFactory;
        }

        protected PluginBase()
        {
            Settings = LoadSettings(null);
        }

        protected PluginBase(string unsecureString, string secureString)
        {
            Settings = LoadSettings(unsecureString);
        }

        protected PluginSettings LoadSettings(string settings)
        {
            var defaultSettings = new PluginSettings()
            {
                LogSettings = new LogSettings()
                {
                    Level = LogLevel.Info
                }
            };

            if (string.IsNullOrWhiteSpace(settings))
            {
                return defaultSettings;
            }

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(PluginSettings));
                var streamArray = System.Text.Encoding.UTF8.GetBytes(settings);
                using (var ms = new MemoryStream(streamArray))
                {
                    var pluginSettings = serializer.ReadObject(ms) as PluginSettings;
                    if (pluginSettings != null)
                    {
                        return pluginSettings;
                    }
                }
            }
            catch
            {
                // Could not deserialize settings, returning default
            }

            return defaultSettings;
        }

        /// <summary>
        /// Gets CRM repositories factory that generates repositories for accessing data in Crm
        /// </summary>
        protected ICrmRepositoriesFactory CrmRepositoriesFactory { get; private set; }

        /// <summary>
        /// Gets CRM services factory that generates services to invoke more complex crm data custom manipulations.
        /// </summary>
        protected ICrmServicesFactory CrmServicesFactory { get; private set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            HandlerCache cache = new HandlerCache();
            IContainer container = CreateContainer(serviceProvider);
            var handlerFactory = new CrmHandlerFactory<TContext>(container, cache);

            IList<ICrmHandler> registeredHandlers = new List<ICrmHandler>();
            RegisterHandlers(handlerFactory, registeredHandlers);

            IList<ICrmWorkHandler> crmHandlers = registeredHandlers.OfType<ICrmWorkHandler>().ToList();
            IList<ICrmValidationHandler> crmValidationHandlers = registeredHandlers.OfType<ICrmValidationHandler>().ToList();

            ProcessValidationHandlers(container, crmValidationHandlers, serviceProvider);
            ProcessHandlers(container, crmHandlers, serviceProvider);
        }

        /// <summary>
        /// Register crm handlers for following plugin, each handler will be invoked if CanWork method will be success.
        /// Handlers are invoked in order as those was added inside plugin.
        /// </summary>
        /// <param name="handlerFactory">The handler factory for generating handlers.</param>
        /// <param name="registeredHandlers">The registered handlers collection of handlers that will be invoked in following plugin execution.</param>
        public abstract void RegisterHandlers(CrmHandlerFactory<TContext> handlerFactory, IList<ICrmHandler> registeredHandlers);

        /// <summary>
        /// Gets tracing service
        /// </summary>
        /// <param name="serviceProvider">Plugin service provider</param>
        /// <returns>Tracing service</returns>
        [Obsolete("Please use IBaseLogger from container")]
        protected ITracingService GetTracingService(IServiceProvider serviceProvider)
        {
            return (ITracingService)serviceProvider.GetService(typeof(ITracingService));
        }

        /// <summary>
        /// Invokes next handlers from argument collection and provides error handling
        /// </summary>
        /// <param name="container">DI Container</param>
        /// <param name="handlers">Collection of handlers implementing ICrmHandler interface.</param>
        /// <param name="serviceProvider">Instance of an object which implements the IServiceProvider interface.</param>
        private void ProcessHandlers(IContainer container, IList<ICrmWorkHandler> handlers, IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var logger = container.Resolve<IBaseLogger>();
            Stopwatch stopwatch = new Stopwatch();

            foreach (ICrmWorkHandler handler in handlers)
            {
                try
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    bool areFiltersFulfilled = FilterAttributesHelper.AreFiltersFulfilled(handler.GetType(), context);

                    if (areFiltersFulfilled)
                    {
                        if (handler.CanWork())
                        {
                            logger.Info($"Can work. Invoking {handler.GetType().Name} handler execution. Context depth: {context.Depth}");
                            handler.DoWork();
                        }
                        else
                        {
                            logger.Info($"Could not work. Ommiting {handler.GetType().Name} handler execution");
                        }
                    }
                    else
                    {
                        logger.Info($"Filters not fulfilled. Ommiting {handler.GetType().Name} handler execution");
                    }
                }
                catch (SecurityException ex)
                {
                    logger.Info($"Plug-in failed with Security Exception: {ex.Message} {ex.StackTrace}");
                    throw new InvalidPluginExecutionException($"Security Exception occured in plugin: {ex.Message} \n {ex.StackTrace}.", ex);
                }
                catch (InvalidPluginExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    TraceExecuteUnexpectedException(logger, ex, nameof(ICrmWorkHandler));
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    logger.Info($"    Execution time: {stopwatch.ElapsedMilliseconds} ms.");
                }
            }
        }

        /// <summary>
        /// Invokes next validation handlers from argument collection and creates message to the end user which contains informations about validation's violations.
        /// </summary>
        /// <param name="container">DI Container</param>
        /// <param name="validationHandlers">Collection of handlers implementing ICrmValidationHandler interface.</param>
        /// <param name="serviceProvider">Instance of an object which implements the IServiceProvider interface.</param>
        private void ProcessValidationHandlers(IContainer container, IList<ICrmValidationHandler> validationHandlers, IServiceProvider serviceProvider)
        {
            IList<string> violations = new List<string>();
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var logger = container.Resolve<IBaseLogger>();

            foreach (ICrmValidationHandler handler in validationHandlers)
            {
                try
                {
                    bool areFiltersFulfilled = FilterAttributesHelper.AreFiltersFulfilled(handler.GetType(), context);

                    if (areFiltersFulfilled)
                    {
                        if (handler.CanWork())
                        {
                            if (!handler.IsValid())
                            {
                                violations.Add(handler.ViolationMessage);
                            }
                            else
                            {
                                logger.Info($"Could not work. Ommiting {handler.GetType().Name} validation handler execution");
                            }
                        }
                    }
                    else
                    {
                        logger.Info($"Filters not fulfilled. Ommiting {handler.GetType().Name} validation handler execution");
                    }
                }
                catch (Exception ex)
                {
                    TraceExecuteUnexpectedException(logger, ex, nameof(ICrmValidationHandler));
                    throw;
                }
            }

            if (violations.Count > 0)
            {
                StringBuilder validationMessage = new StringBuilder();
                foreach (var msg in violations)
                {
                    validationMessage.AppendLine(msg);
                }

                throw new InvalidPluginExecutionException(validationMessage.ToString());
            }
        }

        /// <summary>
        /// Trace using ITracingService passed exception.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="ex">Exception that occured and should be traced.</param>
        private void TraceExecuteUnexpectedException(IBaseLogger logger, Exception ex, string sourceType)
        {
            logger.Error(ex.Message);
            if (ex.InnerException != null)
            {
                logger.Error($"Source: {sourceType}. {ex.InnerException.Message}");
                logger.Error(ex.InnerException.StackTrace);
            }

            logger.Error(ex.StackTrace);
        }

        /// <summary>
        /// Create default DI container for plugins
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider instance</param>
        /// <returns>IContainer instance</returns>
        private IContainer CreateContainer(IServiceProvider serviceProvider)
        {
            IContainer container = new Container();

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var proxyTypesAssembly = context.GetType().GetProperty("ProxyTypesAssembly");
            if (proxyTypesAssembly != null)
            {
                proxyTypesAssembly.SetValue(context, typeof(TContext).Assembly, null);
            }

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            // In profiler ITracingService is replaced by MarshalByRefObject which doesn't work well with Activator
            if (tracingService is MarshalByRefObject)
            {
                tracingService = new ProfilerTracingService();
            }
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var cachedServiceFactory = new CachedOrganizationServiceFactory(serviceFactory);

            container.RegisterSingleton<IOrganizationServiceFactory>(() => cachedServiceFactory);
            container.RegisterSingleton(serviceProvider);
            container.RegisterSingleton<ICrmRepositoriesFactory>(() => CrmRepositoriesFactory ?? new CrmRepositoriesFactory(container));
            container.RegisterSingleton<ICrmServicesFactory>(() => CrmServicesFactory ?? new CrmServicesFactory(container));
            container.RegisterSingleton<ITracingService>(() => tracingService);
            container.Register<IOrganizationService>(() => cachedServiceFactory.CreateOrganizationService(Guid.Empty));
            container.Register<CrmServiceArgs, CrmServiceArgs>();
            container.Register<CrmRepositoryArgs, CrmRepositoryArgs>();
            container.Register<IBaseLogger>(() => new BaseLogger(tracingService, Settings.LogSettings));

            return container;
        }
    }
}