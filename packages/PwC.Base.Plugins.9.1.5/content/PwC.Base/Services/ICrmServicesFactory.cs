namespace PwC.Base.Services
{
    /// <summary>
    /// Crm Service factory interface
    /// </summary>
    public interface ICrmServicesFactory
    {
        /// <summary>
        /// Gets initialized instance of the CrmService
        /// </summary>
        /// <typeparam name="TService">Type of CrmService.</typeparam>
        /// <returns>Initialized object of given CrmService type.</returns>
        TService Get<TService>()
            where TService : ICrmService;
    }
}
