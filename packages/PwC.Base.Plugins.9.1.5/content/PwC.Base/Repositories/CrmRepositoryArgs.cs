using Microsoft.Xrm.Sdk;
using PwC.Base.Log;

namespace PwC.Base.Repositories
{
    public class CrmRepositoryArgs
    {
        public IOrganizationService Service { get; private set; }
        public IBaseLogger Logger { get; private set; }

        public CrmRepositoryArgs(IOrganizationService service, IBaseLogger logger)
        {
            Service = service;
            Logger = logger;
        }
    }
}