using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins.Common.Helpers;

namespace PwC.Base.Plugins.Common.PluginContexts
{
    /// <summary>
    /// Plugins execution context is used inside plugin handlers. It wraps default plugin execution context object
    /// passed inside default CRM plugin. This context is designed to work with Delete message.
    /// </summary>
    /// <typeparam name="T">Entity type that will be a target of deleted entity. Must inherit from Entity type and have parameterless constructor.</typeparam>
    /// <seealso cref="PwC.Plugins.Base.Common.PluginContexts.PluginExecutionContext" />
    public class DeletePluginExecutionContext<T> : PluginExecutionContext
        where T : Entity, new()
    {
        #region Private Fields
        private bool isPreImageLoaded;
        private bool isTargetLoaded;
        private T preImage;
        private EntityReference target;
        #endregion

        internal DeletePluginExecutionContext(IPluginExecutionContext context)
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
        /// Gets the target parameter from plugin execution context input parameters with 'Target' parameter name.
        /// Value is loaded only once to a local variable.
        /// </summary>
        internal EntityReference Target
        {
            get
            {
                if (!this.isTargetLoaded)
                {
                    this.isTargetLoaded = true;
                    this.target = (EntityReference)Context.InputParameters[nameof(Target)];
                }

                return this.target;
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
    }
}