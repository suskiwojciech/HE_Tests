using Microsoft.Xrm.Sdk;

namespace PwC.Base.Plugins.Common.PluginContexts
{
    public class EntityReferencePluginExecutionContext : PluginExecutionContext
    {
        #region Private Fields
        private bool isTargetLoaded = false;
        private EntityReference targetReference = new EntityReference();
        private bool isRelationshipLoaded = false;
        private Relationship relationship = new Relationship();
        private bool isRelatedEntitiesLoaded = false;
        private EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
        #endregion

        public EntityReferencePluginExecutionContext(IPluginExecutionContext context)
            : base(context)
        {
        }

        internal EntityReference TargetReference
        {
            get
            {
                return GetFieldFromContext(ref isTargetLoaded, ref targetReference, "Target");
            }
        }

        internal Relationship Relationship
        {
            get
            {
                return GetFieldFromContext(ref isRelationshipLoaded, ref relationship, nameof(Microsoft.Xrm.Sdk.Relationship));
            }
        }

        internal EntityReferenceCollection RelatedEntities
        {
            get
            {
                return GetFieldFromContext(ref isRelatedEntitiesLoaded, ref relatedEntities, "RelatedEntities");
            }
        }

        private T GetFieldFromContext<T>(ref bool isFieldLoaded, ref T fieldToBeSet, string nameInContext)
        {
            if (!isFieldLoaded)
            {
                isFieldLoaded = true;
                fieldToBeSet = GetInputParameter<T>(nameInContext);
            }

            return fieldToBeSet;
        }
    }
}
