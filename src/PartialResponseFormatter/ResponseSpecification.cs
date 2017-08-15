using System;

namespace PartialResponseFormatter
{
    public class ResponseSpecification
    {
        public Field[] Fields { get; set; }

        public static ResponseSpecification Empty = new ResponseSpecification
        {
            Fields = new Field[0]
        };

        public static ResponseSpecificationBuilder Field(
            string name,
            Func<ResponseSpecificationBuilder, ResponseSpecificationBuilder> innerFieldsFunc = null
            )
        {
            return ResponseSpecificationFactory
                .Create()
                .Field(name, innerFieldsFunc);
        }
        
        public static ResponseSpecification Create<T>()
        {
            return ResponseSpecificationFactory.Create<T>();
        }

        public static implicit operator Field[](ResponseSpecification specification)
        {
            return specification.Fields;
        }

        public static implicit operator ResponseSpecification(Field[] fields)
        {
            return new ResponseSpecification
            {
                Fields = fields
            };
        }
    }
}