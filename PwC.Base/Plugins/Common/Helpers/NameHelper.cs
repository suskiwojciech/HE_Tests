using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PwC.Base.Plugins.Common.Helpers
{
    /// <summary>
    /// Name helper allows to extract certain elements name from their types etc.
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// Gets name of CRM action from request type early bind generated class
        /// </summary>
        /// <param name="actionType">Action request early bind generated class type.</param>
        /// <returns>Name of the action.</returns>
        public static string GetActionNameFromRequest(this Type actionType)
        {
            return actionType.Name.Replace("Request", string.Empty);
        }

        /// <summary>
        /// Gets name of entity attribute name by linq selector
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="T">Selected property type</typeparam>
        /// <param name="selector">Property selector</param>
        /// <returns>Attribute logical name</returns>
        public static string ExtractEntityAttributeName<TEntity, T>(Expression<Func<TEntity, T>> selector)
            where TEntity : Entity, new()
        {
            string attributeName = null;
            var memberExpression = selector.Body as MemberExpression;

            if (memberExpression?.Member != null)
            {
                var memberAttributes = memberExpression
                    .Member
                    .GetCustomAttributes(typeof(AttributeLogicalNameAttribute), inherit: true)
                    .SingleOrDefault() as AttributeLogicalNameAttribute;

                attributeName = memberAttributes?.LogicalName;
            }

            if (string.IsNullOrEmpty(attributeName))
            {
                throw new InvalidPluginExecutionException("Could not find attribute logical name attribute for a defined value changed selector");
            }

            return attributeName;
        }

        /// <summary>
        /// Gets name of relationship attribute name by linq selector
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="T">Selected property type</typeparam>
        /// <param name="selector">Property selector</param>
        /// <returns>Relationship logical name</returns>
        public static string ExtractEntityRelationshipName<TEntity, T>(Expression<Func<TEntity, T>> selector)
            where TEntity : Entity, new()
        {
            string schemaName = null;
            var memberExpression = selector.Body as MemberExpression;

            if (memberExpression?.Member != null)
            {
                var memberAttributes = memberExpression
                    .Member
                    .GetCustomAttributes(typeof(RelationshipSchemaNameAttribute), inherit: true)
                    .SingleOrDefault() as RelationshipSchemaNameAttribute;

                schemaName = memberAttributes?.SchemaName;
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                throw new ArgumentException("Could not find relationship schema name attribute for a defined selector");
            }

            return schemaName;
        }
    }
}
