namespace PwC.Base.Plugins.Common.Constants
{
    /// <summary>
    /// Plugins pipeline processing steps enum corresponds to each step integer representation in Dynamics CRM
    /// </summary>
    public enum CrmProcessingStepStages : int
    {
        Prevalidation = 10,

        Preoperation = 20,

        Postoperation = 40,
    }
}
