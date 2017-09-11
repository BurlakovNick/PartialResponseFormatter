using System;

namespace PartialResponseFormatter
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MapFromContractAttribute : Attribute
    {
        public MapFromContractAttribute(Type dataContractType)
        {
            DataContractType = dataContractType;
        }
            
        public Type DataContractType { get; }
    }
}