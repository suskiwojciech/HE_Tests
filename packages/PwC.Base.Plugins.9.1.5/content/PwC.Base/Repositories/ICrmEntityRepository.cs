using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PwC.Base.Repositories
{
    /// <summary>
    /// CrmEntityRepository interface is a generic interface that exposes basic crm methods to operate on data.
    /// </summary>
    /// <typeparam name="TEntity">The type of the Crm entity.</typeparam>
    /// <typeparam name="TContext">The type of the context inherited from OrganizationServiceCotnext.</typeparam>
    /// <seealso cref="PwC.Base.Repositories.ICrmRepository" />
    public interface ICrmEntityRepository<TEntity, TContext> : ICrmRepository
        where TEntity : Entity
        where TContext : OrganizationServiceContext
    {
        /// <summary>
        /// Adds entity object to a specified Crm queue.
        /// </summary>
        /// <param name="entity">Entity object that will be assigned to given queue.</param>
        /// <param name="destinationQueueId">The destination queue identifier.</param>
        /// <param name="sourceQueueId">Optional source queue identifier.</param>
        /// <returns>Created queue item id of the operation</returns>
        Guid AddToQueue(TEntity entity, Guid destinationQueueId, Guid? sourceQueueId = default(Guid?));

        /// <summary>
        /// Assigns the specified entity object to a new owner.
        /// </summary>
        /// <param name="entity">entity object to update the owner.</param>
        /// <param name="newOwner">Reference to a new owner of the entity object.</param>
        void Assign(TEntity entity, EntityReference newOwner);

        /// <summary>
        /// Assosiate entity with a given entity collection by a given relationship name.
        /// </summary>
        /// <param name="entity">Base entity object to assign related entities to.</param>
        /// <param name="relationshipName">Name of the base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to associate.</param>
        void Associate(TEntity entity, string relationshipName, EntityReferenceCollection relatedEntities);

        /// <summary>
        /// Assosiate entity with a given entity collection by a given relationship.
        /// </summary>
        /// <param name="entity">Base entity object to assign related entities to.</param>
        /// <param name="relationship">Base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to associate.</param>
        void Associate(TEntity entity, Relationship relationship, EntityReferenceCollection relatedEntities);

        /// <summary>
        /// Creates specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        /// <returns>Id of newly generated entity.</returns>
        Guid Create(TEntity entity);

        /// <summary>
        /// Deletes the specified entity object from crm organization.
        /// </summary>
        /// <param name="entity">Entity object to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Disassociate entity with a given entity collection by a given relationship name.
        /// </summary>
        /// <param name="entity">Base entity object to dissasociate related entities from.</param>
        /// <param name="relationshipName">Name of the base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to disassociate.</param>
        void Disassociate(TEntity entity, string relationshipName, EntityReferenceCollection relatedEntities);

        /// <summary>
        /// Disassociate entity with a given entity collection by a given relationship.
        /// </summary>
        /// <param name="entity">Base entity object to dissasociate related entities from.</param>
        /// <param name="relationship">Base entity relationship.</param>
        /// <param name="relatedEntities">The related entities collection to disassociate.</param>
        void Disassociate(TEntity entity, Relationship relationship, EntityReferenceCollection relatedEntities);

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
        IList<TEntity> GetByAttribute(string attributeName, object value, string[] columns = null, int? limitResult = null);

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Only specified columns are gathered.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <param name="columns">Entity columns to gather.</param>
        /// <returns>Crm entity object value with only specified columns.</returns>
        TEntity GetById(Guid id, params string[] columns);

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Specified selector is applied at the end.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <param name="selector">Entity selector to apply.</param>
        /// <returns>
        /// Crm entity object value with specified selector applied.
        /// </returns>
        TEntity GetById(Guid id, Expression<Func<TEntity, TEntity>> selector = null);

        /// <summary>
        /// Gets entity by a given identifier from the crm organization.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>Crm entity object value</returns>
        TEntity GetById(EntityReference id, params string[] columns);

        /// <summary>
        /// Gets entity by a given identifier from the crm organization. Returns null if not found.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// Crm entity or null.
        /// </returns>
        TEntity GetByIdOrDefault(Guid? id);

        /// <summary>
        /// Grants access to an entity object for a given principal (user or team)
        /// </summary>
        /// <param name="entity">Entity object for granting access.</param>
        /// <param name="principal">Security principal reference to grant access to (user or team).</param>
        /// <param name="accessRights">The access rights to grant to a principal.</param>
        void GrantAccess(TEntity entity, EntityReference principal, AccessRights accessRights);

        /// <summary>
        /// Revokes access to an entity object for a given principal (user or team)
        /// </summary>
        /// <param name="entity">Entity object for revoking access.</param>
        /// <param name="principal">Security principal reference to grant access to (user or team).</param>
        void RevokeAccess(TEntity entity, EntityReference principal);

        /// <summary>
        /// Set state of multiple records
        /// </summary>
        /// <param name="entity">Records of specified entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        void SetState(IList<TEntity> entities, Enum stateCode, Enum statusCode);

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entity">Entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        void SetState(TEntity entity, int stateCode, int statusCode);

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entity">Entity that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        void SetState(TEntity entity, Enum stateCode, Enum statusCode);

        /// <summary>
        /// Updates the specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Retrieves multiple entities
        /// </summary>
        IList<TEntity> RetrieveAll();

        /// <summary>
        /// Checks if entity with a given id exists
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// True if entity found, false otherwise
        /// </returns>
        bool Exists(Guid id);
    }

    /// <summary>
    /// CrmEntityRepository interface is interface that exposes basic crm methods to operate on data.
    /// </summary>
    /// <seealso cref="PwC.Plugins.Base.Repositories.ICrmRepository" />
    public interface ICrmEntityRepository : ICrmRepository
    {
        /// <summary>
        /// Creates specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        /// <returns>Id of newly generated entity.</returns>
        Guid Create(Entity entity);

        /// <summary>
        /// Deletes the specified entity object from crm organization.
        /// </summary>
        /// <param name="entityId">EntityReference to delete.</param>
        void Delete(EntityReference entityId);

        /// <summary>
        /// Sets state of an entity object.
        /// </summary>
        /// <param name="entityId">Entity reference that state will be changed.</param>
        /// <param name="stateCode">New state code for the entity object.</param>
        /// <param name="statusCode">New status code for the entity object.</param>
        void SetState(EntityReference entityId, int stateCode, int statusCode);

        /// <summary>
        /// Updates the specified entity object in crm organization.
        /// </summary>
        /// <param name="entity">Entity object to update.</param>
        void Update(Entity entity);

        /// <summary>
        /// Checks if entity with a given id exists
        /// </summary>
        /// <param name="entityName">Entity logical name.</param>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>
        /// True if entity found, false otherwise
        /// </returns>
        bool Exists(string entityName, Guid id);

        /// <summary>
        /// Gets entity by a given identifier from the crm organization.
        /// </summary>
        /// <param name="id">Crm Entity object identifier.</param>
        /// <returns>Crm entity object value</returns>
        Entity GetById(EntityReference id, params string[] columns);
    }
}