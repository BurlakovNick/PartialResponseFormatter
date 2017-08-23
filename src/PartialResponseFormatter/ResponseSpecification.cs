using System;
using System.Text;

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

        public static bool CheckClientMatchesServer<TClientData, TServerData>(
            out FieldMismatch[] fieldsMismatches)
        {
            //todo: write tests
            fieldsMismatches = ResponseSpecificationMatcher.FindClientFieldMismatches<TClientData, TServerData>();
            return fieldsMismatches.Length == 0;
        }

        public static bool CheckClientMatchesServer<TServerData>(ResponseSpecification clientResponseSpecification,
            out FieldMismatch[] fieldsMismatcher)
        {
            //todo: implement
            fieldsMismatcher = new FieldMismatch[0];
            return fieldsMismatcher.Length == 0;
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