using Newtonsoft.Json;
using SwaggerWcf.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace SwaggerWcf.AutoGenerate.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Type serviceContractType = System.IO.Directory
                .GetFiles("../../../SwaggerWcf.Test.Service/bin", "*.dll")
                .Select(System.IO.Path.GetFullPath)
                .Select(Assembly.LoadFile)
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => type.GetCustomAttribute<ServiceContractAttribute>() != null)
                .Select(type => new
                {
                    Type = type,
                    HasSwaggerWcfAttribute = type.GetCustomAttribute<SwaggerWcfAttribute>() != null
                })
                .Where(typeInfo => typeInfo.HasSwaggerWcfAttribute)
                .Select(typeInfo => typeInfo.Type)
                .Single()
            ;

            var schema = new SwaggerSchemaBuilder(serviceContractType).Build();
            string json = JsonConvert.SerializeObject(schema, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            System.IO.File.WriteAllText(@"C:\swagger.json", json);
        }
    }
}
