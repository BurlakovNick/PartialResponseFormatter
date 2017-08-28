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

        /// <summary>
        /// Use for a client validation when server contract is known as a CLR type
        /// </summary>
        public static bool CheckClientMatchesServer<TClientData, TServerData>(
            out FieldMismatch[] fieldsMismatches)
        {
            fieldsMismatches = ResponseSpecificationMatcher.FindClientFieldMismatches<TClientData, TServerData>();
            return fieldsMismatches.Length == 0;
        }

        /// <summary>
        /// Use for a server validation when usually client expected CLR type is unknown
        /// </summary>
        public static bool CheckClientMatchesServer<TServerData>(ResponseSpecification clientResponseSpecification,
            out FieldMismatch[] fieldsMismatches)
        {
            fieldsMismatches = ResponseSpecificationMatcher.FindClientFieldMismatches<TServerData>(clientResponseSpecification);
            return fieldsMismatches.Length == 0;
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