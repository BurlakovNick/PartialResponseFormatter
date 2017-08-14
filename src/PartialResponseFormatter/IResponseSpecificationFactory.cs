namespace PartialResponseFormatter
{
    public interface IResponseSpecificationFactory
    {
        ResponseSpecification Create<T>();
    }
}