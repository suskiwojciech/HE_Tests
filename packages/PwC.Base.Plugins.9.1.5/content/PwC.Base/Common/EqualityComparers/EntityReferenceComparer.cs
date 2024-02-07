using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace PwC.Base.Common.EqualityComparers
{
    public class EntityReferenceComparer : IEqualityComparer<EntityReference>
    {
        public bool Equals(EntityReference x, EntityReference y)
        {
            return x.Id == y.Id && x.LogicalName == y.LogicalName;
        }

        public int GetHashCode(EntityReference obj)
        {
            return obj.GetHashCode();
        }
    }
}
