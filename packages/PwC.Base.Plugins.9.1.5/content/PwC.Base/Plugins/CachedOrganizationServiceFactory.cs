using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Concurrent;

namespace PwC.Base.Plugins
{
    /// <summary>
    /// Cached organization serviced factory to avoid 'exceeded' exception.
    /// Exception occures where to many crm client are created in crm plugin.
    /// </summary>
    public class CachedOrganizationServiceFactory : IOrganizationServiceFactory
    {
        private readonly ConcurrentDictionary<Guid, IOrganizationService> organizationServicesCache = new ConcurrentDictionary<Guid, IOrganizationService>();
        private readonly IOrganizationServiceFactory crmOrganizationServiceFactory;

        private object factoryLock = new object();
        private IOrganizationService systemService = null;

        private IOrganizationService SystemService
        {
            get
            {
                if (systemService != null)
                {
                    return systemService;
                }
                else
                {
                    lock (factoryLock)
                    {
                        if (systemService != null)
                        {
                            return systemService;
                        }
                        else
                        {
                            return systemService = crmOrganizationServiceFactory.CreateOrganizationService(null);
                        }
                    }
                }
            }
        }

        public CachedOrganizationServiceFactory(IOrganizationServiceFactory crmOrganizationServiceFactory)
        {
            this.crmOrganizationServiceFactory = crmOrganizationServiceFactory;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return SystemService;
            }
            else
            {
                return this.organizationServicesCache.GetOrAdd(userId.Value, id => this.crmOrganizationServiceFactory.CreateOrganizationService(id));
            }
        }
    }
}