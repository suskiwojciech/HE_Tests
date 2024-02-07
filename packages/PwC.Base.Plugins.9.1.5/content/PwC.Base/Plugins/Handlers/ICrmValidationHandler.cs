namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Crm Validation Handler interface describes how validation plugin handler should look like and which work method should implement.
    /// </summary>
    public interface ICrmValidationHandler : ICrmHandler
    {
        string ViolationMessage { get; }

        /// <summary>
        /// Do Validation method should implement logic that should validate certain scenario.
        /// In this method logic should access crm data base on Crm services and repositories. Those are accessable by
        /// CrmRepositoriesFactory and CrmServicesFactory.
        /// ExecutionData contains plugin context data to process.
        /// </summary>
        bool IsValid();
    }
}
