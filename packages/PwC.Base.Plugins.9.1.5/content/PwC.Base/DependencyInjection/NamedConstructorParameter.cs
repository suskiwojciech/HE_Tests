using System;
using System.Reflection;

namespace PwC.Base.DependencyInjection
{
    public class NamedConstructorParameter : ConstructorParameter
    {
        public NamedConstructorParameter(string name, object instance)
            : base(instance)
        {
            this.Name = name;
        }

        public string Name { get; protected set; }

        public override bool IsValid(ParameterInfo parameter)
        {
            return string.Equals(parameter.Name, Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
