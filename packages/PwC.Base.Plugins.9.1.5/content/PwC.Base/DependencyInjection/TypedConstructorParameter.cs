using System;
using System.Reflection;

namespace PwC.Base.DependencyInjection
{
    public class TypedConstructorParameter : ConstructorParameter
    {
        public TypedConstructorParameter(Type serviceType, object instance)
            : base(instance)
        {
            this.ServiceType = serviceType;
        }

        public Type ServiceType { get; protected set; }

        public override bool IsValid(ParameterInfo parameter)
        {
            return parameter.ParameterType.Equals(ServiceType);
        }
    }
}
