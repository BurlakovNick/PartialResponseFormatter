namespace RestPartialResponse
{
    public interface IResponseSpecificationSerializer
    {
        string Serialize(ResponseSpecification specification);
        ResponseSpecification Deserialize(string specificationString);
    }
}