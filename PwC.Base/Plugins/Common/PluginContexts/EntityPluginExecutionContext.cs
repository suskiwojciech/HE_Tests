using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins.Common.Constants;
using PwC.Base.Plugins.Common.Helpers;
using PwC.Base.Common.Extensions;

namespace PwC.Base.Plugins.Common.PluginContexts
{
    /// <summary>
    /// Plugins execution context is used inside plugin handlers. It wraps default plugin execution context object
    /// passed inside default CRM plugin. This context is designed to work with messages that pass Entity as target.
    /// (create, update, etc. operations)
    /// </summary>
    /// <typeparam name="T">Entity type that will be a Target of a context. Must inherit from Entity type and have parameterless constructor.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Common.PluginContexts.PluginExecutionContext" />
    public class EntityPluginExecutionContext<T> : PluginExecutionContext
        where T : Entity, new()
    {
        #region Private Fields
        private T preImage;
        private bool isPreImageLoaded;
        private T postImage;
        private bool isPostImageLoaded;
        private T target;
        private bool isTargetLoaded;
        #endregion

        internal EntityPluginExecutionContext(IPluginExecutionContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the pre image from plugin execution context images with 'PreImage' image name.
        /// </summary>
        internal T PreImage
        {
            get
            {
                return ImageHelper<T>.GetEntityImage(nameof(this.PreImage), Context.PreEntityImages, ref this.preImage, ref this.isPreImageLoaded);
            }
        }

        /// <summary>
        /// Gets the post image from plugin execution context images with 'PostImage' image name.
        /// </summary>
        internal T PostImage
        {
            get
            {
                return ImageHelper<T>.GetEntityImage(nameof(this.PostImage), Context.PostEntityImages, ref this.postImage, ref this.isPostImageLoaded);
            }
        }

        /// <summary>
        /// Gets the target parameter from plugin execution context input parameters with 'Target' parameter name.
        /// Value is loaded only once to a local variable.
        /// </summary>
        internal T Target
        {
            get
            {
                if (!this.isTargetLoaded)
                {
                    this.isTargetLoaded = true;
                    this.target = ((Entity)Context.InputParameters[nameof(Target)]).ToEntity<T>();
                }

                return this.target;
            }
        }

        /// <summary>
        /// Gets current entity state combining fields values from both target and preimage values.
        /// Entity PreImage must exists for this operation.
        /// </summary>
        /// <returns>Current entity state object value with combined attributes values.</returns>
        /// <exception cref="InvalidPluginExecutionException">Cannot retrieve entity current state. PreImage does not exists</exception>
        internal T GetCurrentState()
        {
            if (PreImageExists())
            {
                return PreImage.Merge(Target);
            }
            else
            {
                throw new InvalidPluginExecutionException("Cannot retrieve entity current state. PreImage does not exists");
            }
        }

        /// <summary>
        /// Checks if Target input paraemter exists in plugin execution context.
        /// </summary>
        /// <returns>Boolean value indicating if a Target parameter exists.</returns>
        internal bool TargetExists()
        {
            if (Context.InputParameters.Contains(nameof(Target)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if PreImage images paraemter exists in plugin execution context.
        /// </summary>
        /// <returns>Boolean value indicating if a PreImage parameter exists.</returns>
        internal bool PreImageExists()
        {
            if (this.PreImage != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if PostImage images paraemter exists in plugin execution context.
        /// </summary>
        /// <returns>Boolean value indicating if a PostImage parameter exists.</returns>
        internal bool PostImageExists()
        {
            if (this.PostImage != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if specified field changed
        /// </summary>
        /// <param name="field">Field name</param>
        /// <returns>True if field was changed</returns>
        internal bool ValueChanged(string field)
        {
            if (!TargetExists())
            {
                return false;
            }

            if (Target.Contains(field))
            {
                if (IsMessage(CrmMessage.Create))
                {
                    return true;
                }
                else if (IsMessage(CrmMessage.Update))
                {
                    if (!PreImageExists())
                    {
                        throw new InvalidPluginExecutionException("PreImage is required for update handlers.");
                    }

                    if (!PreImage.Contains(field))
                    {
                        return true;
                    }

                    return !object.Equals(PreImage[field], Target[field]);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if specified field changed to the specified value
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="newValue">Expected value</param>
        /// <returns>True if field was changed</returns>
        internal bool ValueChanged(string field, object newValue)
        {
            bool isCreate = IsMessage(CrmMessage.Create);
            bool isUpdate = IsMessage(CrmMessage.Update);

            if (!TargetExists() && !(isCreate || isUpdate) && !Target.Contains(field))
            {
                return false;
            }

            // Convert enum to optionset values
            newValue = ConvertEnumToOptionSetValue(newValue);
            bool isNewValueProper = object.Equals(Target.GetAttributeValue<object>(field), newValue);

            if (isNewValueProper)
            {
                if (isCreate)
                {
                    return true;
                }

                if (isUpdate)
                {
                    if (!PreImageExists())
                    {
                        throw new InvalidPluginExecutionException("PreImage is required for update handlers.");
                    }

                    var oldValue = PreImage.GetAttributeValue<object>(field);
                    bool isOldValueDifferent = !object.Equals(oldValue, newValue);

                    // Old value should be different from the new one
                    return isOldValueDifferent;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if specified field changed from oldValue to the newValue
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="oldValue">Old valuie</param>
        /// <param name="newValue">New value</param>
        /// <returns>True if field was changed</returns>
        internal bool ValueChanged(string field, object oldValue, object newValue)
        {
            if (!TargetExists() || IsMessage(CrmMessage.Create))
            {
                return false;
            }

            // Convert enum to optionset values
            oldValue = ConvertEnumToOptionSetValue(oldValue);
            newValue = ConvertEnumToOptionSetValue(newValue);

            if (Target.Contains(field) && IsMessage(CrmMessage.Update))
            {
                if (!PreImageExists())
                {
                    throw new InvalidPluginExecutionException("PreImage is required for update handlers.");
                }

                if (!PreImage.Contains(field))
                {
                    return true;
                }

                return object.Equals(PreImage.GetAttributeValue<object>(field), oldValue) &&
                    object.Equals(Target.GetAttributeValue<object>(field), newValue);
            }

            return false;
        }

        /// <summary>
        /// If input is a enum it is converted to the opitonsetvalue.
        /// </summary>
        /// <param name="couldBeEnumObject">Specified attribute (could be enum)</param>
        /// <returns>Optionsetvalue or specified input (if not enum)</returns>
        private object ConvertEnumToOptionSetValue(object couldBeEnumObject)
        {
            if (couldBeEnumObject != null && couldBeEnumObject.GetType().IsEnum)
            {
                return new OptionSetValue((int)couldBeEnumObject);
            }
            else
            {
                return couldBeEnumObject;
            }
        }
    }
}