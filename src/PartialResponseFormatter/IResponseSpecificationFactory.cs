namespace RestPartialResponse
{
    public interface IResponseSpecificationFactory
    {
        ResponseSpecification Create<T>();
    }
}