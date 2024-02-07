namespace PwC.Base.Plugins.Handlers
{
    /// <summary>
    /// Crm Entity Handler interface describes how entity plugin handler should look like and which work method should implement.
    /// </summary>
    public interface ICrmWorkHandler : ICrmHandler
    {
        /// <summary>
        /// Do Work method should implement logic that should invoke certain scenario for handler.
        /// In this method logic should access crm data base on Crm services and repositories. Those are accessable by
        /// CrmRepositoriesFactory and CrmServicesFactory.
        /// ExecutionData contains plugin context data to process.
        /// </summary>
        void DoWork();
    }
}
