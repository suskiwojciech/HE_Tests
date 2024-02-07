using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace PwC.Base.Common.Extensions
{
    /// <summary>
    /// Extensions to help with change of existing queries
    /// </summary>
    public static class QueryExpressionExtensions
    {
        public static LinkEntity GetOrCreate(this QueryExpression query,
            string linkToEntityName,
            string linkFromAttributeName,
            string linkToAttributeName,
            JoinOperator? joinOperator = null)
        {
            var link = query.LinkEntities.FindLink(linkToEntityName, linkFromAttributeName, linkToAttributeName);

            if (link != null)
            {
                if (!joinOperator.HasValue || link.JoinOperator == joinOperator)
                {
                    return link;
                }
                else
                {
                    query.LinkEntities.Remove(link);
                }
            }

            return query.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator ?? JoinOperator.Inner);
        }

        public static LinkEntity GetOrCreate(this LinkEntity parentLink,
            string linkToEntityName,
            string linkFromAttributeName,
            string linkToAttributeName,
            JoinOperator? joinOperator = null)
        {
            var link = parentLink.LinkEntities.FindLink(linkToEntityName, linkFromAttributeName, linkToAttributeName);

            if (link != null)
            {
                if (!joinOperator.HasValue || link.JoinOperator == joinOperator)
                {
                    return link;
                }
                else
                {
                    parentLink.LinkEntities.Remove(link);
                }
            }

            return parentLink.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator ?? JoinOperator.Inner);
        }

        public static LinkEntity FindLink(this DataCollection<LinkEntity> linkEntities,
            string linkToEntityName,
            string linkFromAttributeName,
            string linkToAttributeName)
        {
            return linkEntities.FirstOrDefault(l =>
                l.LinkToEntityName == linkToEntityName
                && l.LinkFromAttributeName == linkFromAttributeName
                && l.LinkToAttributeName == linkToAttributeName);
        }

        public static void ClearColumns(this QueryExpression query)
        {
            query.ColumnSet = new ColumnSet(false);
            foreach (var linkEntity in query.LinkEntities)
            {
                linkEntity.ClearColumns();
            }
        }

        public static void ClearColumns(this LinkEntity link)
        {
            link.Columns = new ColumnSet(false);
            foreach (var linkEntity in link.LinkEntities)
            {
                linkEntity.ClearColumns();
            }
        }

        public static FilterExpression PackExisitngCriteriaIntoNewFilter(this FilterExpression criteria, LogicalOperator filterOperator = LogicalOperator.And)
        {
            var viewConditions = criteria.Conditions.ToList();
            var viewFilterOperator = criteria.FilterOperator;
            var viewFilters = criteria.Filters.ToList();

            criteria.Filters.Clear();
            criteria.Conditions.Clear();
            criteria.FilterOperator = filterOperator;

            // Existing conditions
            if (viewFilters.Any() || viewConditions.Any())
            {
                var subFilter = criteria.AddFilter(viewFilterOperator);

                if (viewFilters.Any())
                {
                    subFilter.Filters.AddRange(viewFilters);
                }
                if (viewConditions.Any())
                {
                    subFilter.Conditions.AddRange(viewConditions);
                }
            }

            return criteria;
        }
    }
}