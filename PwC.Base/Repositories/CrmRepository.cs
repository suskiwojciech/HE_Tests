using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using PwC.Base.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// Base Crm repository class that implements ICrmRepository interface
    /// </summary>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Base.Repositories.CrmRepository" />
    public class CrmRepository<TContext> : CrmRepository
        where TContext : OrganizationServiceContext
    {
        protected readonly TContext xrmContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmRepository{TContext}"/> class.
        /// </summary>
        /// <param name="service">CrmRepositoryArgs class to operate on dynamics crm.</param>
        public CrmRepository(CrmRepositoryArgs args)
            : base(args)
        {
            this.xrmContext = CreateContext(MergeOption.NoTracking);
        }

        /// <summary>
        /// Creates the OrganizationServiceContext of a declared type passing there organizationservice
        /// </summary>
        /// <param name="merge">Optional merge option parameter that will be set on the generated context.</param>
        /// <returns>Declared crm organization context class where queries can be created.</returns>
        protected TContext CreateContext(MergeOption? merge = null)
        {
            var context = (TContext)Activator.CreateInstance(typeof(TContext), service);
            if (merge.HasValue)
            {
                context.MergeOption = merge.Value;
            }

            return context;
        }
    }

    /// <summary>
    /// Base Crm repository class that implements ICrmRepository interface. Non-generic, without XrmContext version.
    /// </summary>
    /// <seealso cref="PwC.Base.Repositories.ICrmRepository" />
    public class CrmRepository : ICrmRepository
    {
        protected readonly IOrganizationService service;
        protected readonly IBaseLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmRepository"/> class.
        /// </summary>
        /// <param name="args">CrmRepositoryArgs class to operate on dynamics crm.</param>
        public CrmRepository(CrmRepositoryArgs args)
        {
            this.service = args.Service;
            this.logger = args.Logger;
        }

        /// <summary>
        /// Executes the specified request on the Crm organizationservice.
        /// </summary>
        /// <typeparam name="VRequest">The type of the crm request.</typeparam>
        /// <typeparam name="TResponse">The type of the crm response.</typeparam>
        /// <param name="request">Reqeust object that will be executed.</param>
        /// <returns>
        /// Request response of a given type
        /// </returns>
        public TResponse Execute<VRequest, TResponse>(VRequest request)
            where VRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            using (TraceExecution(request))
            {
                return (TResponse)this.service.Execute(request);
            }
        }

        /// <summary>
        /// Executes specified requests in transactional batches
        /// </summary>
        /// <param name="requests">Requests to execute</param>
        /// <param name="pageSize">Batch page size</param>
        /// <returns>Responses</returns>
        public OrganizationResponseCollection ExecuteAllRequestsInTransactionalBatches(IEnumerable<OrganizationRequest> requests, int pageSize = 100)
        {
            using (TraceExecution(requests, pageSize))
            {
                int page = 1;
                var result = new OrganizationResponseCollection();

                IEnumerable<OrganizationRequest> pageData;
                while ((pageData = requests.Skip((page++ - 1) * pageSize).Take(pageSize)).Any())
                {
                    ExecuteTransactionRequest req = new ExecuteTransactionRequest()
                    {
                        Requests = new OrganizationRequestCollection()
                    };
                    req.Requests.AddRange(pageData);

                    var response = Execute<ExecuteTransactionRequest, ExecuteTransactionResponse>(req);
                    result.AddRange(response.Responses);
                }

                return result;
            }
        }

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="queryExpression">Query expression</param>
        /// <param name="pageSize">Number of records per page. Optional. Default value is 5000</param>
        /// <returns>EntityCollection</returns>
        public virtual EntityCollection RetrieveAll(QueryExpression queryExpression, int pageSize = 5000)
        {
            using (TraceExecution(queryExpression, pageSize))
            {
                List<Entity> entities = new List<Entity>();
                EntityCollection result;

                if (!queryExpression.TopCount.HasValue)
                {
                    queryExpression.PageInfo = new PagingInfo
                    {
                        Count = pageSize,
                        PageNumber = 1
                    };
                }
                else
                {
                    queryExpression.PageInfo = new PagingInfo();
                }

                do
                {
                    result = this.service.RetrieveMultiple(queryExpression);

                    queryExpression.PageInfo.PageNumber++;
                    queryExpression.PageInfo.PagingCookie = result.PagingCookie;
                    entities.AddRange(result.Entities);
                }
                while (result.MoreRecords);

                return new EntityCollection(entities);
            }
        }

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="queryExpression">Query by attribute expression</param>
        /// <param name="pageSize">Number of records per page. Optional. Default value is 5000</param>
        /// <returns>EntityCollection</returns>
        public EntityCollection RetrieveAll(QueryByAttribute queryExpression, int pageSize = 5000)
        {
            using (TraceExecution(queryExpression, pageSize))
            {
                List<Entity> entities = new List<Entity>();
                EntityCollection result = new EntityCollection();

                if (!queryExpression.TopCount.HasValue)
                {
                    queryExpression.PageInfo = new PagingInfo
                    {
                        Count = pageSize,
                        PageNumber = 1
                    };
                }
                else
                {
                    queryExpression.PageInfo = new PagingInfo();
                }

                do
                {
                    result = this.service.RetrieveMultiple(queryExpression);

                    queryExpression.PageInfo.PageNumber++;
                    queryExpression.PageInfo.PagingCookie = result.PagingCookie;
                    entities.AddRange(result.Entities);
                }
                while (result.MoreRecords);

                return new EntityCollection(entities);
            }
        }

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <param name="fetchXml">FetchXml</param>
        /// <returns>EntityCollection</returns>
        public virtual EntityCollection RetrieveAll(string fetchXml)
        {
            using (TraceExecution(fetchXml))
            {
                return this.service.RetrieveMultiple(new FetchExpression(fetchXml));
            }
        }

        /// <summary>
        /// Convert FetchXml to QueryExpression
        /// </summary>
        /// <param name="fetchXml">FetchXml string</param>
        /// <returns>QueryExpression</returns>
        public virtual QueryExpression FetchXmlToQueryExpression(string fetchXml)
        {
            using (TraceExecution(fetchXml))
            {
                var req = new FetchXmlToQueryExpressionRequest() { FetchXml = fetchXml };
                var resp = (FetchXmlToQueryExpressionResponse)this.service.Execute(req);

                return resp.Query;
            }
        }

        /// <summary>
        /// Convert QueryExpression to FetchXml
        /// </summary>
        /// <param name="query">QueryExpression object</param>
        /// <returns>FetchXml string</returns>
        public virtual string QueryExpressionToFetchXml(QueryExpression query)
        {
            using (TraceExecution(query))
            {
                var req = new QueryExpressionToFetchXmlRequest() { Query = query };
                var resp = (QueryExpressionToFetchXmlResponse)this.service.Execute(req);

                return resp.FetchXml;
            }
        }

        /// <summary>
        /// Trace and logs execution time of selected code block by using statement
        /// </summary>
        /// <returns>Disposable trace log context</returns>
        protected TraceLogContext TraceExecution(params object[] parameters)
        {
            return TraceLogContext.Create(logger, parameters);
        }
    }
}