using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public class PartialResponseFormatter : IPartialResponseFormatter
    {
        public object Format(object obj, ResponseSpecification responseSpecification)
        {
            return Traverse(obj, responseSpecification.Fields);
        }

        //todo: slow, i know. IL-code will arrive soon
        private static object Traverse(object obj, Field[] fields)
        {
            if (obj == null)
            {
                return null;
            }
            
            if (fields.IsEmpty())
            {
                return obj;
            }

            var dictionaryObj = obj as IDictionary;
            if (dictionaryObj != null)
            {
                return TraverseDictionary(dictionaryObj, fields);
            }

            var enumerableObj = obj as IEnumerable;
            if (enumerableObj != null)
            {
                return TraverseEnumerable(enumerableObj, fields);
            }

            var result = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                var property = ReflectionProvider.GetPropertyValue(obj, field.Name);
                if (property == null)
                {
                    continue;
                }
                
                var subfields = field.Fields;
                result.Add(field.Name, Traverse(property, subfields));
            }
            
            return result;
        }

        private static List<object> TraverseEnumerable(IEnumerable enumerableObj, Field[] fields)
        {
            return enumerableObj
                .Cast<object>()
                .Select(element => Traverse(element, fields))
                .Where(formattedElement => formattedElement != null)
                .ToList();
        }

        private static Dictionary<object, object> TraverseDictionary(IDictionary dictionaryObj, Field[] fields)
        {
            var result = new Dictionary<object, object>();
            foreach (DictionaryEntry kvp in dictionaryObj)
            {
                var formattedElement = Traverse(kvp.Value, fields);
                if (formattedElement != null)
                {
                    result.Add(kvp.Key, formattedElement);
                }
            }
            return result;
        }
    }
}