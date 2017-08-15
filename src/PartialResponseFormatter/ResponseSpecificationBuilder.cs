using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponseFormatter
{
    public class ResponseSpecificationBuilder
    {
        private readonly ResponseSpecificationBuilder parentBuilder;
        private ResponseSpecificationBuilder innerFieldsBuilder;
        private string fieldName;

        public ResponseSpecificationBuilder(ResponseSpecificationBuilder parentBuilder = null)
        {
            this.parentBuilder = parentBuilder;
        }

        public ResponseSpecificationBuilder Field(
            string name,
            ResponseSpecificationBuilder innerFields
        )
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name can't be empty");
            }

            fieldName = name;
            innerFieldsBuilder = innerFields;

            return new ResponseSpecificationBuilder(this);
        }
        
        public ResponseSpecificationBuilder Field(
            string name, 
            Func<ResponseSpecificationBuilder, ResponseSpecificationBuilder> innerFieldsFunc = null
        )
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name can't be empty");
            }

            fieldName = name;
            innerFieldsBuilder = innerFieldsFunc?.Invoke(new ResponseSpecificationBuilder());

            return new ResponseSpecificationBuilder(this);
        }
        
        public ResponseSpecification Create()
        {
            var fields = new List<Field>();
            var currentBuilder = parentBuilder;
            
            while (currentBuilder != null)
            {
                var name = currentBuilder.fieldName;
                var subfields = currentBuilder.innerFieldsBuilder?.Create().Fields ?? new Field[0];
                
                fields.Add(new Field
                {
                    Name = name,
                    Fields = subfields
                });
                currentBuilder = currentBuilder.parentBuilder;
            }

            fields.Reverse();
            
            return new ResponseSpecification
            {
                Fields = fields.ToArray()
            };
        }

        public static implicit operator ResponseSpecification(ResponseSpecificationBuilder builder)
        {
            return builder.Create();
        }
    }
}