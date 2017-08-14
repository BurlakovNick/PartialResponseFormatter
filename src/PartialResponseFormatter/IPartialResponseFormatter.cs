using System.Collections.Generic;

namespace PartialResponseFormatter
{
    public interface IPartialResponseFormatter
    {
        object Format(object obj, ResponseSpecification responseSpecification);
    }
}