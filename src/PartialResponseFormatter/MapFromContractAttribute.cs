using System;

namespace PartialResponseFormatter
{
    //todo: should use later for fields binding validation
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