using System.Linq;
using System.Reflection;

namespace PartialResponseFormatter
{
    public static class FieldNameProvider
    {
        public static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var customAttributes = propertyInfo.GetCustomAttributes().ToArray();
            var jsonPropertyAttribute = customAttributes.FirstOrDefault(a => a.GetType().Name == "JsonPropertyAttribute");
            if (jsonPropertyAttribute != null)
            {
                var jsonPropertyName = jsonPropertyAttribute
                    .GetType()
                    .GetProperty("PropertyName", BindingFlags.Instance | BindingFlags.Public);
                if (jsonPropertyName != null)
                {
                    var customName = (string) jsonPropertyName.GetValue(jsonPropertyAttribute);
                    if (!string.IsNullOrEmpty(customName))
                    {
                        return customName;
                    }
                }
            }

            var partialResponsePropertyAttribute = customAttributes.FirstOrDefault(a => a.GetType() == typeof(PartialResponsePropertyAttribute));
            if (partialResponsePropertyAttribute != null)
            {
                var propertyAttribute = (PartialResponsePropertyAttribute)partialResponsePropertyAttribute;
                var customName = propertyAttribute.PropertyName;
                if (!string.IsNullOrEmpty(customName))
                {
                    return customName;
                }
            }

            return propertyInfo.Name;
        }
    }
}