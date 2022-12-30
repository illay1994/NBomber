using NBomber.Contracts;
using System;
using System.Collections.Generic;

namespace NBomber.Runner.Abstraction
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MarkAssemblyAttribure : Attribute
    {
        public MarkAssemblyAttribure(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(ISetup).IsAssignableFrom(type))
            {
                throw new ArgumentException($@"""{type}"" does not implement {typeof(ISetup)}.", nameof(type));
            }
            Type = type;
        }

        public Type Type { get; }
    }

    public interface ISetup
    {
        IEnumerable<ScenarioProps> Setup();
    }
}
