using System.Collections.Generic;

namespace PartialResponseFormatter
{
    public interface IPartialResponseFormatter
    {
        Dictionary<string, object> Format(object obj, ResponseSpecification responseSpecification);
    }
}