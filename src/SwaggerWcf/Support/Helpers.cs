using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using SwaggerWcf.Configuration;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal static class Helpers
    {
        public static TypeFormat MapSwaggerType(Type type, IList<Type> definitions = null)
        {
            //built-in types
            if (type == typeof(bool))
            {
                return new TypeFormat(ParameterType.Boolean, null);
            }
            if (type == typeof(byte))
            {
                return new TypeFormat(ParameterType.String, "byte");
            }
            if (type == typeof(sbyte))
            {
                return new TypeFormat(ParameterType.String, "sbyte");
            }
            if (type == typeof(char))
            {
                return new TypeFormat(ParameterType.String, "char");
            }
            if (type == typeof(decimal))
            {
                return new TypeFormat(ParameterType.Number, "decimal");
            }
            if (type == typeof(double))
            {
                return new TypeFormat(ParameterType.Number, "double");
            }
            if (type == typeof(float))
            {
                return new TypeFormat(ParameterType.Number, "float");
            }
            if (type == typeof(int))
            {
                return new TypeFormat(ParameterType.Integer, "int32");
            }
            if (type == typeof(uint))
            {
                return new TypeFormat(ParameterType.Number, "uint32");
            }
            if (type == typeof(long))
            {
                return new TypeFormat(ParameterType.Integer, "int64");
            }
            if (type == typeof(ulong))
            {
                return new TypeFormat(ParameterType.Integer, "uint64");
            }
            if (type == typeof(short))
            {
                return new TypeFormat(ParameterType.Integer, "int16");
            }
            if (type == typeof(ushort))
            {
                return new TypeFormat(ParameterType.Integer, "uint16");
            }
            if (type == typeof(string))
            {
                return new TypeFormat(ParameterType.String, null);
            }
            if (type == typeof(DateTime))
            {
                return new TypeFormat(ParameterType.String, "date-time");
            }
            if (type == typeof(DateTimeOffset))
            {
                return new TypeFormat(ParameterType.String, "date-time");
            }
            if (type == typeof(Guid))
            {
                return new TypeFormat(ParameterType.String, "guid");
            }
            if (type == typeof(Stream))
            {
                return new TypeFormat(ParameterType.String, "stream");
            }

            if (type == typeof(void))
            {
                return new TypeFormat(ParameterType.Void, null);
            }

            //it's an enum, use string as the property type and enum values will be serialized later
            if (type.IsEnum)
            {
                return new TypeFormat(ParameterType.Integer, "enum");
            }

            //it's a collection/array, so it will use the swagger "container" syntax
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return new TypeFormat(ParameterType.Array, null);
            }

            //it's a collection/array, so it will use the swagger "container" syntax
            if (type.GetInterfaces().Any(i => i == typeof(System.Collections.IEnumerable)))
            {
                return new TypeFormat(ParameterType.Array, null);
            }

            //it's a Nullable<T> so treat it as T - we'll handle making it not-required later
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return MapSwaggerType(type.GenericTypeArguments[0], definitions);
            }

            //it's a complex type, so we'll need to map it later
            if (definitions != null && !definitions.Contains(type))
            {
                definitions.Add(type);
            }

            return new TypeFormat(ParameterType.Object, HttpUtility.HtmlEncode(type.GetModelName()));
        }

        private static string BuildTypeString(string typeName, string defaultNote = null, string typeNote = null)
        {
            const string resultFormat = "{0}({1})";

            if (string.IsNullOrEmpty(defaultNote) && string.IsNullOrEmpty(typeNote))
            {
                return typeName;
            }

            return string.IsNullOrEmpty(typeNote)
                       ? string.Format(resultFormat, typeName, defaultNote)
                       : string.Format(resultFormat, typeName, typeNote);
        }

        public static T1 GetCustomAttributeValue<T1, T2>(MethodInfo method, string propertyName)
            where T1 : class
            where T2 : Attribute
        {
            var attr = method.GetCustomAttribute<T2>();
            if (attr == null)
            {
                return null;
            }

            PropertyInfo prop = typeof(T2).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(T1))
            {
                return null;
            }

            return prop.GetValue(attr) as T1;
        }

        public static bool GetCustomAttributeValue<T>(MethodInfo method, string propertyName, bool defaultVal = false)
            where T : Attribute
        {
            var attr = method.GetCustomAttribute<T>();
            if (attr == null)
            {
                return defaultVal;
            }

            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            if (prop == null || prop.PropertyType != typeof(bool))
            {
                return defaultVal;
            }

            return (bool)prop.GetValue(attr);
        }

        internal static TypeFormat MapElementType(Type type, List<Type> definitions)
        {
            Type enumType =
                type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumType == null)
            {
                throw new ArgumentException("Type must be an IEnumerable<T>.");
            }

            Type elementType = enumType.GetGenericArguments().First();

            return MapSwaggerType(elementType, definitions);
        }

        internal static bool TagIsHidden(this Dictionary<string, TagElement> tagConfigurations,
                                         IEnumerable<string> itemTags)
        {
            return tagConfigurations.Values.Any(t => t.Visibile.Equals(false) && itemTags.Contains(t.Name));
        }
    }
}
