using Microsoft.Xrm.Sdk;
using PwC.Base.Plugins.Common.Constants;

namespace PwC.Base.Plugins.Common.PluginContexts
{
    /// <summary>
    /// Plugins execution context is used inside plugin handlers. It wraps default plugin execution context object
    /// passed inside default CRM plugin.
    /// </summary>
    public class PluginExecutionContext
    {
        public PluginExecutionContext(IPluginExecutionContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets default plugin execution context passed into CRM plugin.
        /// It contains all services and providers for manipulating data, etc.
        /// </summary>
        public IPluginExecutionContext Context { get; private set; }

        /// <summary>
        /// Gets the input parameter of a given name from the
        /// plugin execution context input parameters collection.
        /// </summary>
        /// <typeparam name="V">Value type of a parameter that will be gathered from parameters collection.</typeparam>
        /// <param name="key">Name of the parameter in Input parameters collection.</param>
        /// <returns>Value of a given parameter key and projects it on selected type.</returns>
        public V GetInputParameter<V>(string key)
        {
            if (this.Context.InputParameters.ContainsKey(key))
            {
                return (V)this.Context.InputParameters[key];
            }

            return default(V);
        }

        /// <summary>
        /// Gets the output parameter of a given name from the
        /// plugin execution context output parameters collection.
        /// </summary>
        /// <typeparam name="V">Value type of a parameter that will be gathered from parameters collection.</typeparam>
        /// <param name="key">Name of the parameter in Output parameters collection.</param>
        /// <returns>Value of a given parameter key and projects it on selected type.</returns>
        public V GetOutputParameter<V>(string key)
        {
            if (this.Context.OutputParameters.ContainsKey(key))
            {
                return (V)this.Context.OutputParameters[key];
            }

            return default(V);
        }

        /// <summary>
        /// Sets the input parameter of a given name in the
        /// plugin execution context input parameters collection.
        /// </summary>
        /// <typeparam name="V">Value type of a parameter that will be inserted to parameters collection</typeparam>
        /// <param name="key">Name of the parameter in input parameters collection.</param>
        /// <param name="value">Value of a given parameter key of a given type.</param>
        public void SetInputParameter<V>(string key, V value)
        {
            this.Context.InputParameters[key] = value;
        }

        /// <summary>
        /// Sets the output parameter of a given name in the
        /// plugin execution context output parameters collection.
        /// </summary>
        /// <typeparam name="V">Value type of a parameter that will be inserted to parameters collection</typeparam>
        /// <param name="key">Name of the parameter in output parameters collection.</param>
        /// <param name="value">Value of a given parameter key of a given type.</param>
        public void SetOutputParameter<V>(string key, V value)
        {
            if (this.Context.OutputParameters.ContainsKey(key))
            {
                this.Context.OutputParameters[key] = value;
            }
            else
            {
                this.Context.OutputParameters.Add(key, value);
            }
        }

        /// <summary>
        /// Compares specified message with plugin execution context message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>True if equal</returns>
        public bool IsMessage(CrmMessage message)
        {
            return IsMessage(message.ToString());
        }

        /// <summary>
        /// Compares specified message with plugin execution context message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>True if equal</returns>
        public bool IsMessage(string message)
        {
            return Context.MessageName == message;
        }
    }
}