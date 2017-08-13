using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestPartialResponse
{
    //todo: to string builder
    public class UrlParameterResponseSpecificationSerializer : IResponseSpecificationSerializer
    {
        public string Serialize(ResponseSpecification specification)
        {
            return string.Join(";", specification.Fields.Select(Serialize));
        }

        private string Serialize(Field field)
        {
            if (field.Fields == null || field.Fields.Length == 0)
            {
                return field.Name;
            }

            var fields = string.Join(";", field.Fields.Select(Serialize));
            return $"{field.Name}:({fields})";
        }

        public ResponseSpecification Deserialize(string specificationString)
        {
            var tokens = Tokenize(specificationString);
            var position = 0;
            var fields = Deserialize(tokens, ref position);
            return new ResponseSpecification
            {
                Fields = fields
            };
        }

        private Field[] Deserialize(string[] tokens, ref int position)
        {
            var fields = new List<Field>();
            for (;position < tokens.Length; )
            {
                while (position < tokens.Length && tokens[position] == ";")
                {
                    position++;
                }

                if (tokens[position] == ")")
                {
                    position++;
                    break;
                }
                
                var name = tokens[position++];
                if (HasSubfields(tokens, position))
                {
                    SkipToken(":", tokens, ref position);
                    SkipToken("(", tokens, ref position);
                    var subfields = Deserialize(tokens, ref position);
                    fields.Add(new Field
                    {
                        Name = name,
                        Fields = subfields.ToArray()
                    });
                }
                else
                {
                    fields.Add(new Field
                    {
                        Name = name,
                        Fields = new Field[0]
                    });
                }
            }
            return fields.ToArray();
        }

        private bool HasSubfields(string[] tokens, int position)
        {
            return tokens.Length > position && tokens[position] == ":";
        }

        private void SkipToken(string expected, string[] tokens, ref int position)
        {
            if (tokens[position] != expected)
            {
                throw new InvalidOperationException($"Expected token {expected} but was {tokens[position]}");
            }
            position++;
        }

        //todo: need strong types
        private string[] Tokenize(string specificationString)
        {
            var tokens = new List<string>();
            var currentToken = new StringBuilder();
            foreach (var character in specificationString)
            {
                if (character == ':' || character == '(' || character == ')' || character == ';')
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());                        
                    }
                    tokens.Add(character.ToString());
                    currentToken.Clear();
                    continue;
                }

                currentToken.Append(character);
            }

            if (currentToken.Length > 0)
            {
                tokens.Add(currentToken.ToString());                
            }
            return tokens.Concat(new [] {")"}).ToArray();
        }
    }
}