using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestPartialResponse
{
    public class PartialResponseFormatter : IPartialResponseFormatter
    {
        //todo: doesn't work with collections in root
        public Dictionary<string, object> Format(object obj, ResponseSpecification responseSpecification)
        {
            return Format(obj, responseSpecification.Fields);
        }

        //todo: slow, i know. IL-code will arrive soon
        private Dictionary<string, object> Format(object obj, Field[] fields)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name);
            var result = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                PropertyInfo property;
                if (!properties.TryGetValue(field.Name, out property))
                {
                    throw new InvalidOperationException();
                }

                var value = property.GetValue(obj);
                var subfields = field.Fields;
                if (subfields.Length > 0)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.GetInterface("IDictionary") != null)
                    {
                        var dictionary = (IDictionary)value;
                        var elements = new Dictionary<object, object>();
                        //todo: check if type is simple
                        foreach (DictionaryEntry keyValuePair in dictionary)
                        {
                            var formattedValue = Format(keyValuePair.Value, subfields);
                            elements.Add(keyValuePair.Key, formattedValue);
                        }
                        result.Add(field.Name, elements);
                    }
                    else
                    if (propertyType.GetInterface("IEnumerable") != null)
                    {
                        var array = (IEnumerable)value;
                        var elements = new List<object>();
                        foreach (var arrayElement in array)
                        {
                            elements.Add(Format(arrayElement, subfields));
                        }
                        result.Add(field.Name, elements);
                    }
                    else
                    {
                        result.Add(field.Name, Format(value, subfields));
                    }
                }
                else
                {
                    result.Add(field.Name, value);
                }
            }
            return result;
        }
    }
}