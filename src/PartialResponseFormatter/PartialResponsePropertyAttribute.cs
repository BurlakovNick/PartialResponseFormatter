using System;

namespace PartialResponseFormatter
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PartialResponsePropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }
    }
}