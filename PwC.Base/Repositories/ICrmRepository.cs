using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// Base, general Crm Repository interface
    /// </summary>
    public interface ICrmRepository
    {
        /// <summary>
        /// Executes the specified request on the Crm organizationservice.
        /// </summary>
        /// <typeparam name="VRequest">The type of the crm request.</typeparam>
        /// <typeparam name="TResponse">The type of the crm response.</typeparam>
        /// <param name="request">Reqeust object that will be executed.</param>
        /// <returns>Request response of a given type</returns>
        TResponse Execute<VRequest, TResponse>(VRequest request)
            where VRequest : OrganizationRequest
            where TResponse : OrganizationResponse;

        /// <summary>
        /// Executes specified requests in transactional batches
        /// </summary>
        /// <param name="requests">Requests to execute</param>
        /// <param name="pageSize">Batch page size</param>
        /// <returns>Responses</returns>
        OrganizationResponseCollection ExecuteAllRequestsInTransactionalBatches(IEnumerable<OrganizationRequest> requests, int pageSize = 100);


        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="queryExpression">Query expression</param>
        /// <param name="pageSize">Number of records per page. Optional. Default value is 5000</param>
        /// <returns>EntityCollection</returns>
        EntityCollection RetrieveAll(QueryExpression queryExpression, int pageSize = 5000);

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="queryExpression">Query by attribute expression</param>
        /// <param name="pageSize">Number of records per page. Optional. Default value is 5000</param>
        /// <returns>EntityCollection</returns>
        EntityCollection RetrieveAll(QueryByAttribute queryExpression, int pageSize = 5000);

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="fetchXml">FetchXml</param>
        /// <returns>EntityCollection</returns>
        EntityCollection RetrieveAll(string fetchXml);

        /// <summary>
        /// Convert FetchXml to QueryExpression
        /// </summary>
        /// <param name="fetchXml">FetchXml string</param>
        /// <returns>QueryExpression</returns>
        QueryExpression FetchXmlToQueryExpression(string fetchXml);

        /// <summary>
        /// Convert QueryExpression to FetchXml
        /// </summary>
        /// <param name="query">QueryExpression object</param>
        /// <returns>FetchXml string</returns>
        string QueryExpressionToFetchXml(QueryExpression query);
    }
}
