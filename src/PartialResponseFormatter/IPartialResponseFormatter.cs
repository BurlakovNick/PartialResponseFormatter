using System.Collections.Generic;

namespace RestPartialResponse
{
    public interface IPartialResponseFormatter
    {
        Dictionary<string, object> Format(object obj, ResponseSpecification responseSpecification);
    }
}