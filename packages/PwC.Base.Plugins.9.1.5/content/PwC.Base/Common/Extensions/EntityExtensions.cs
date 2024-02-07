using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PwC.Base.Common.Extensions
{
    public static class EntityExtensions
    {
        public static T Merge<T>(this T older, T newer)
            where T : Entity, new()
        {
            T merged = new T();
            merged.LogicalName = older.LogicalName;

            foreach (var attribute in older.Attributes)
            {
                merged[attribute.Key] = older[attribute.Key];
            }

            if (newer != null && newer is Entity)
            {
                foreach (var attribute in newer.Attributes)
                {
                    merged[attribute.Key] = newer[attribute.Key];
                }
            }

            // Entity.Id is not in the Attributes collection so we must set it explicit.
            if (Guid.Empty != older.Id)
            {
                merged.Id = older.Id;
            }

            if (Guid.Empty != newer.Id)
            {
                merged.Id = newer.Id;
            }

            return merged;
        }

        public static T GetAliasedAttributeValue<T>(this Entity entity, string alias, string fieldName)
        {
            var aliasedValue = entity.GetAttributeValue<AliasedValue>($"{alias}.{fieldName}");

            var castType = typeof(T);
            Type enumType;

            if (aliasedValue != null)
            {
                if (aliasedValue.Value is T)
                {
                    return (T)aliasedValue.Value;
                }
                else if ((enumType = GetEnumTypeOrDefault(castType)) != null
                    && aliasedValue.Value is OptionSetValue
                    && Enum.IsDefined(enumType, ((OptionSetValue)aliasedValue.Value)?.Value))
                {
                    return (T)Enum.ToObject(enumType, ((OptionSetValue)aliasedValue.Value)?.Value);
                }
                else
                {
                    throw new InvalidCastException($@"Could not cast aliased value type '{aliasedValue.Value.GetType().Name}' 
                        to the expected '{typeof(T).Name}' type.");
                }
            }

            return default(T);
        }

        public static string GetFormattedValue(this Entity entity, string fieldName)
        {
            string result = string.Empty;

            if (entity.Attributes.Contains(fieldName))
            {
                if (entity.FormattedValues.Contains(fieldName))
                {
                    result = entity.FormattedValues[fieldName];
                }
                else
                {
                    var attribute = entity.Attributes[fieldName];
                    if (attribute.GetType() == typeof(AliasedValue))
                    {
                        result = ((AliasedValue)attribute).Value.ToString();
                    }
                    else
                    {
                        result = entity.Attributes[fieldName].ToString();
                    }
                }
            }

            return result;
        }

        public static EntityReference ToEntityReference<E>(this string that)
            where E : Entity, new()
        {
            if (Guid.TryParse(that, out Guid referenceId))
            {
                string logicalName = (new E()).LogicalName;
                return new EntityReference(logicalName, referenceId);
            }
            return null;
        }

        public static EntityReference ToEntityReference<E>(this Guid? that)
            where E : Entity, new()
        {
            if (that.HasValue && that.Value != Guid.Empty)
            {
                return that.Value.ToEntityReference<E>();
            }
            return null;
        }

        public static EntityReference ToEntityReference<E>(this Guid that)
            where E : Entity, new()
        {
            string logicalName = (new E()).LogicalName;
            return new EntityReference(logicalName, that);
        }

        public static Guid? ToGuidOrDefault(this string that)
        {
            if (Guid.TryParse(that, out Guid guid))
            {
                return guid;
            }
            return null;
        }

        private static Type GetEnumTypeOrDefault(Type type)
        {
            Type underlyingType;
            if (type is null || type.IsEnum)
            {
                return type;
            }
            else if ((underlyingType = Nullable.GetUnderlyingType(type)) != null)
            {
                if (underlyingType.IsEnum)
                {
                    return underlyingType;
                }
            }
            return null;
        }

        public static T ToEntity<T>(this Entity entity, string alias) where T : Entity, new()
        {
            var aliasedEntity = new T();
            if (!entity.Attributes.Any())
            {
                return null;
            }
            var aliasedFields = entity.Attributes.Keys.Where(x => x.StartsWith($"{alias}."));
            if (!aliasedFields.Any())
            {
                return null;
            }
            foreach (var aliasedField in aliasedFields)
            {
                var aliasedValue = entity.GetAttributeValue<AliasedValue>(aliasedField);
                if (aliasedValue != null)
                {
                    aliasedEntity[aliasedField.Split('.').Last()] = aliasedValue.Value;
                }
            }
            var formattedFields = entity.FormattedValues.Keys.Where(x => x.StartsWith($"{alias}."));
            foreach (var formattedField in formattedFields)
            {
                aliasedEntity.FormattedValues[formattedField.Remove(0, $"{alias}.".Length)] = entity.FormattedValues[formattedField];
            }

            if (aliasedEntity.Contains(aliasedEntity.LogicalName + "id"))
            {
                aliasedEntity.Id = (Guid)aliasedEntity[aliasedEntity.LogicalName + "id"];
            }

            return aliasedEntity;
        }

        public static ICollection<T> ToEntities<T>(this EntityCollection entityCollection, string alias = "") where T : Entity, new()
        {
            List<T> entities = new List<T>();
            if (entityCollection == null || entityCollection.Entities == null || !entityCollection.Entities.Any())
                return entities;
            foreach (var entity in entityCollection.Entities)
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    entities.Add(entity.ToEntity<T>());
                }
                else
                {
                    entities.Add(entity.ToEntity<T>(alias));
                }
            }
            return entities;
        }
    }
}
