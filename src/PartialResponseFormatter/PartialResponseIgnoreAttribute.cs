using System;

namespace PartialResponseFormatter
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PartialResponseIgnoreAttribute : Attribute
    {
    }
}