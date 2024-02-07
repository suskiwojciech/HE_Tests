using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using PwC.Base.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// Generic Crm Entity repository class that implements ICrmEntityRepository interface.
    /// It exposes basic Crm operations to invoke on crm organization context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the Crm entity.</typeparam>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Base.Repositories.CrmRepository{TContext}" />
    /// <seealso cref="PwC.Base.Repositories.ICrmEntityRepository{TEntity, TContext}" />
    public class CrmEntityRepository<TEntity, TContext> : CrmRepository<TContext>, ICrmEntityRepository<TEntity, TContext>
        where TEntity : Entity, new()
        where TContext : OrganizationServiceContext
    {
        private const string CannotFindEntityLogicalNameMessage = "Cannot find entity logical name";

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmEntityRepository{TEntity, TContext}"/> class.
        /// </summary>
        /// <param name="service">CrmRepositoryArgs class to operate on dynamics crm.</param>
        public CrmEntityRepository(CrmRepositoryArgs args)
            : base(args)
        {
        }

        private string LogicalName
        {
            get
            {
                var field = typeof(TEntity).GetField("EntityLogicalName");
                if (field != null && field.IsStatic)
                {
                    return (string)field.GetValue(null); // field is static.
                }
                else
                {
                    throw new Exception(CannotFindEntityLogicalNameMessage);
                }
            }
        }

        /// <summary>
        /// Adds entity object to a specified Crm queue.
        /// </summary>
        /// <param name="entity">Entity object that will be assigned to given queue.</param>
        /// <param name="destinationQueueId">The destination queue identifier.</param>
        /// <param name="sourceQueueId">Optional source queue identifier.</param>
        /// <returns>
        /// Created queue item id of the operation
        /// </returns>
        public virtual Guid AddToQueue(TEntity entity, Guid destinationQueueId, Guid? sourceQueueId = default(Guid?))
        {
            using (TraceExecution(entity, destinationQueueId, sourceQueueId))
            {
                var addToQueueRequest = new AddToQueueRequest();
                if (sourceQueueId.HasValue)
                {
                    addToQueueRequest.SourceQueueId = sourceQueueId.Value;
                }

                addToQueueRequest.Target = entity.ToEntityReference();
                addToQueueRequest.DestinationQueueId = destinationQueueId;

                AddToQueueResponse response = (AddToQueueResponse)this.service.Execute(addToQueueRequest);
                return response.QueueItemId;
            }
        }

        /// <summary>
        /// Assigns the specified entity object to a new owner.
        /// </summary>
        /// <param name="entity">entity object to update the owner.</param>
        /// <param name="newOwner">Reference to a new owner of the entity object.</param>
        public virtual void Assign(TEntity entity, EntityReference newOwner)
        {
            using (TraceExecution(entity, newOwner))
            {
                this.service.Execute(new AssignRequest()
                {
                    Target = entity.ToEntityReference(),
                    Assignee = newOwner
                });
            }
        }

        /// <summary>
        /// Assosiate entity with a given entity collection by a given relationship.
        /// </summary>
        /// <param name="entity">Base entity object to assign related entities to.</param>
        /// <param name="relationship">Base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to associate.</param>
        public virtual void Associate(TEntity entity, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            using (TraceExecution(entity, relationship, relatedEntities))
            {
                this.service.Associate(entity.LogicalName, entity.Id, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// Assosiate entity with a given entity collection by a given relationship name.
        /// </summary>
        /// <param name="entity">Base entity object to assign related entities to.</param>
        /// <param name="relationshipName">Name of the base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to associate.</param>
        public virtual void Associate(TEntity entity, string relationshipName, EntityReferenceCollection relatedEntities)
        {
            using (TraceExecution(entity, relationshipName, relatedEntities))
            {
                this.Associate(entity, new Relationship(relationshipName), relatedEntities);
            }
        }

        /// <summary>
        /// Creates specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        /// <returns>
        /// Id of newly generated entity.
        /// </returns>
        public virtual Guid Create(TEntity entity)
        {
            using (TraceExecution(entity))
            {
                return this.service.Create(entity);
            }
        }

        /// <summary>
        /// Deletes the specified entity object from crm organization.
        /// </summary>
        /// <param name="entity">Entity object to delete.</param>
        public virtual void Delete(TEntity entity)
        {
            using (TraceExecution(entity))
            {
                this.service.Delete(entity.LogicalName, entity.Id);
            }
        }

        /// <summary>
        /// Disassociate entity with a given entity collection by a given relationship.
        /// </summary>
        /// <param name="entity">Base entity object to dissasociate related entities from.</param>
        /// <param name="relationship">Base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to disassociate.</param>
        public virtual void Disassociate(TEntity entity, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            using (TraceExecution(entity, relationship, relatedEntities))
            {
                this.service.Disassociate(entity.LogicalName, entity.Id, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// Disassociate entity with a given entity collection by a given relationship name.
        /// </summary>
        /// <param name="entity">Base entity object to dissasociate related entities from.</param>
        /// <param name="relationshipName">Name of the base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to disassociate.</param>
        public virtual void Disassociate(TEntity entity, string relationshipName, EntityReferenceCollection relatedEntities)
        {
            using (TraceExecution(entity, relationshipName, relatedEntities))
            {
                this.Disassociate(entity, new Relationship(relationshipName), relatedEntities);
            }
        }

        /// <summary>
        /// Searches crm organization context for a given entity and matches all records where given attribute have a given value assigned.
        /// </summary>
        /// <param name="attributeName">Name of <seealso cref="!:TEntity" /> entity attribute.</param>
        /// <param name="value">The value of an attribute.</param>
        /// <param name="columns">Optional set of columns to retrieve. All columns will be retrieved if not specified.</param>
        /// <param name="limitResult">Optional results limit value.</param>
        /// <returns>
        /// List collection of entity object fits the given attribute value.
        /// </returns>
        public IList<TEntity> GetByAttribute(string attributeName, object value, string[] columns = null, int? limitResult = null)
        {
            using (TraceExecution(attributeName, value, columns, limitResult))
            {
                var query = new QueryByAttribute();

                query.EntityName = new TEntity().LogicalName;
                query.ColumnSet = columns is null ? new ColumnSet(true) : new ColumnSet(columns);
                query.TopCount = limitResult;
                query.Attributes.Add(attributeName);
                query.Values.Add(value);

                return RetrieveAll(query)
                    .Entities
                    .Select(e => e.ToEntity<TEntity>())
                    .ToList();
            }
        }

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Specified selector is applied at the end.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <param name="selector">Entity selector to apply.</param>
        /// <returns>
        /// Crm entity object value with specified selector applied.
        /// </returns>
        public virtual TEntity GetById(Guid id, Expression<Func<TEntity, TEntity>> selector = null)
        {
            using (TraceExecution(id, selector))
            {
                var query = this.xrmContext.CreateQuery<TEntity>();

                if (selector != null)
                {
                    return query.Where(q => q.Id == id).Select(selector).Single();
                }

                return query.Where(q => q.Id == id).Single();
            }
        }

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Returns null if not found.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// Crm entity or null.
        /// </returns>
        public virtual TEntity GetByIdOrDefault(Guid? id)
        {
            using (TraceExecution(id))
            {
                if (!id.HasValue) return null;

                var query = this.xrmContext.CreateQuery<TEntity>();

                return query.Where(q => q.Id == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Only specified columns are gathered.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <param name="columns">Entity columns to gather.</param>
        /// <returns>
        /// Crm entity object value with only specified columns.
        /// </returns>
        public virtual TEntity GetById(Guid id, params string[] columns)
        {
            using (TraceExecution(id, columns))
            {
                return this.service.Retrieve(
                           this.LogicalName,
                           id,
                           columns != null && columns.Any() ? new ColumnSet(columns) : new ColumnSet(true))
                           .ToEntity<TEntity>();
            }
        }

        /// <summary>
        /// Gets entity by a given identifier from the crm organization.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>Crm entity object value</returns>
        public virtual TEntity GetById(EntityReference id, params string[] columns)
        {
            using (TraceExecution(id, columns))
            {
                return service.Retrieve(
                        this.LogicalName,
                        id.Id,
                        columns != null && columns.Any() ? new ColumnSet(columns) : new ColumnSet(true))
                        .ToEntity<TEntity>();
            }
        }

        /// <summary>
        /// Grants access to an entity object for a given principal (user or team)
        /// </summary>
        /// <param name="entity">Entity object for granting access.</param>
        /// <param name="principal">Security principal reference to grant access to (user or team).</param>
        /// <param name="accessRights">The access rights to grant to a principal.</param>
        public virtual void GrantAccess(TEntity entity, EntityReference principal, AccessRights accessRights)
        {
            using (TraceExecution(entity, principal, accessRights))
            {
                this.service.Execute(new GrantAccessRequest()
                {
                    Target = entity.ToEntityReference(),
                    PrincipalAccess = new PrincipalAccess()
                    {
                        Principal = principal,
                        AccessMask = accessRights,
                    }
                });
            }
        }

        // TODO Merge with RetrieveAll(fetch), Task #24980
        public EntityCollection FetchPagedData(string fetchXml)
        {
            using (TraceExecution(fetchXml))
            {
                EntityCollection returnCollection = new EntityCollection();
                int pageNumber = 1;
                string pagingCookie = string.Empty;
                EntityCollection singleFetchResult = null;

                do
                {
                    singleFetchResult = FetchPageData(fetchXml, pageNumber, pagingCookie);
                    returnCollection.Entities.AddRange(singleFetchResult.Entities);
                    pagingCookie = singleFetchResult.PagingCookie;
                    pageNumber++;
                }
                while (singleFetchResult.MoreRecords);

                return returnCollection;
            }
        }

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        /// <returns>EntityCollection</returns>
        public virtual IList<TEntity> RetrieveAll()
        {
            using (TraceExecution())
            {
                string logicalName = new TEntity().LogicalName;
                QueryExpression query = new QueryExpression(logicalName);
                query.ColumnSet = new ColumnSet(allColumns: true);

                return RetrieveAll(query).Entities.Select(e => e.ToEntity<TEntity>()).ToList();
            }
        }

        /// <summary>
        /// Revokes access to an entity object for a given principal (user or team)
        /// </summary>
        /// <param name="entity">Entity object for revoking access.</param>
        /// <param name="principal">Security principal reference to grant access to (user or team).</param>
        public virtual void RevokeAccess(TEntity entity, EntityReference principal)
        {
            using (TraceExecution(entity, principal))
            {
                this.service.Execute(new RevokeAccessRequest()
                {
                    Target = entity.ToEntityReference(),
                    Revokee = principal,
                });
            }
        }

        /// <summary>
        /// Set state of multiple records
        /// </summary>
        /// <param name="entity">Records of specified entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        public virtual void SetState(IList<TEntity> entities, Enum stateCode, Enum statusCode)
        {
            using (TraceExecution(entities, stateCode, statusCode))
            {
                var recordsToUpdate = entities.Select(entity => new UpdateRequest
                {
                    Target = new Entity
                    {
                        Id = entity.Id,
                        LogicalName = entity.LogicalName,
                        [EntityCommonFieldsNames.StateCode] = new OptionSetValue(Convert.ToInt32(stateCode)),
                        [EntityCommonFieldsNames.StatusCode] = new OptionSetValue(Convert.ToInt32(statusCode)),
                    }
                });
                this.ExecuteAllRequestsInTransactionalBatches(recordsToUpdate);
            }
        }

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entity">Entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        public virtual void SetState(TEntity entity, int stateCode, int statusCode)
        {
            using (TraceExecution(entity, stateCode, statusCode))
            {
                var toUpdate = new Entity() { Id = entity.Id, LogicalName = entity.LogicalName };

                toUpdate[EntityCommonFieldsNames.StateCode] = new OptionSetValue(stateCode);
                toUpdate[EntityCommonFieldsNames.StatusCode] = new OptionSetValue(statusCode);

                this.service.Update(toUpdate);
            }
        }

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entity">Entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        public virtual void SetState(TEntity entity, Enum stateCode, Enum statusCode)
        {
            using (TraceExecution(entity, stateCode, statusCode))
            {
                SetState(entity, Convert.ToInt32(stateCode), Convert.ToInt32(statusCode));
            }
        }

        /// <summary>
        /// Updates the specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object to update.</param>
        public void Update(TEntity entity)
        {
            using (TraceExecution(entity))
            {
                this.service.Update(entity);
            }
        }

        /// <summary>
        /// Checks if entity with a given id exists
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// True if entity found, false otherwise
        /// </returns>
        public virtual bool Exists(Guid id)
        {
            using (TraceExecution(id))
            {
                string entityName = new TEntity().LogicalName;

                var query = new QueryExpression(entityName);
                query.TopCount = 1;
                query.Criteria.AddCondition(entityName + "id", ConditionOperator.Equal, id);
                query.ColumnSet = new ColumnSet(entityName + "id");

                var result = this.service.RetrieveMultiple(query);

                return result.Entities.Any();
            }
        }

        private EntityCollection FetchPageData(string fetchXml, int page, string pagingCookie)
        {
            if (pagingCookie != null && pagingCookie != string.Empty)
            {
                pagingCookie = pagingCookie.Replace("\"", "'").Replace(">", "&gt;").Replace("<", "&lt;");
            }

            fetchXml = string.Format(fetchXml, page, pagingCookie);
            var qe = new FetchExpression(fetchXml);
            var result = service.RetrieveMultiple(qe);
            return result;
        }
    }

    /// <summary>
    /// Generic Crm Entity repository class that implements ICrmEntityRepository interface.
    /// It exposes basic Crm operations to invoke on crm organization service.
    /// </summary>
    public class CrmEntityRepository : CrmRepository, ICrmEntityRepository
    {
        public CrmEntityRepository(CrmRepositoryArgs args)
            : base(args)
        {
        }

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entity">Entity that will be created.</param>
        public Guid Create(Entity entity)
        {
            using (TraceExecution(entity))
            {
                return this.service.Create(entity);
            }
        }

        /// <summary>
        /// Deletes the specified entity object from crm organization.
        /// </summary>
        /// <param name="entityId">EntityReference to delete.</param>
        public void Delete(EntityReference entityId)
        {
            using (TraceExecution(entityId))
            {
                if (entityId == null)
                {
                    throw new ArgumentNullException(nameof(entityId));
                }

                service.Delete(entityId.LogicalName, entityId.Id);
            }
        }

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entityId">Entity reference that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        public void SetState(EntityReference entityId, int stateCode, int statusCode)
        {
            using (TraceExecution(entityId, stateCode, statusCode))
            {
                if (entityId == null)
                {
                    throw new ArgumentNullException(nameof(entityId));
                }

                var entity = new Entity()
                {
                    LogicalName = entityId.LogicalName,
                    Id = entityId.Id
                };

                entity[EntityCommonFieldsNames.StateCode] = new OptionSetValue(stateCode);
                entity[EntityCommonFieldsNames.StatusCode] = new OptionSetValue(statusCode);

                service.Update(entity);
            }
        }

        /// <summary>
        /// Updates the specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object to update.</param>
        public void Update(Entity entity)
        {
            using (TraceExecution(entity))
            {
                service.Update(entity);
            }
        }

        /// <summary>
        /// Checks if entity with a given id exists
        /// </summary>
        /// <param name="entityName">Entity logical name.</param>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// True if entity found, false otherwise
        /// </returns>
        public bool Exists(string entityName, Guid id)
        {
            using (TraceExecution(entityName, id))
            {
                var query = new QueryExpression(entityName);
                query.TopCount = 1;
                query.Criteria.AddCondition(entityName + "id", ConditionOperator.Equal, id);
                query.ColumnSet = new ColumnSet(entityName + "id");

                var result = this.service.RetrieveMultiple(query);

                return result.Entities.Any();
            }
        }

        /// <summary>
        /// Gets entity by a given identifier from the crm organization.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>Crm entity object value</returns>
        public virtual Entity GetById(EntityReference id, params string[] columns)
        {
            using (TraceExecution(id, columns))
            {
                return service.Retrieve(
                        id.LogicalName,
                        id.Id,
                        columns != null && columns.Any() ? new ColumnSet(columns) : new ColumnSet(true));
            }
        }
    }
}