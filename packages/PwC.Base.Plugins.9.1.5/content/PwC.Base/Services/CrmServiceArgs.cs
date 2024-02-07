using Microsoft.Xrm.Sdk;
using PwC.Base.Log;
using PwC.Base.Repositories;

namespace PwC.Base.Services
{
    public class CrmServiceArgs
    {
        public readonly ICrmRepositoriesFactory CrmRepositoriesFactory;
        public readonly ITracingService TracingService;
        public readonly ICrmServicesFactory CrmServicesFactory;
        public readonly IBaseLogger Logger;

        public CrmServiceArgs(ICrmRepositoriesFactory crmRepositoriesFactory,
                              ICrmServicesFactory crmServicesFactory,
                              ITracingService tracingService,
                              IBaseLogger logger)
        {
            this.CrmRepositoriesFactory = crmRepositoriesFactory;
            this.CrmServicesFactory = crmServicesFactory;
            this.TracingService = tracingService;
            this.Logger = logger;
        }
    }
}
