using SwaggerWcf.Attributes;
using SwaggerWcf.Models;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace SwaggerWcf.AutoGenerate.Test
{
    public class SwaggerSchemaBuilder
    {
        private readonly Type _serviceContractType;

        private static Type[] _requiredAttributeTypes = new[]
        {
            typeof(ServiceContractAttribute),
            typeof(SwaggerWcfAttribute),
            typeof(SwaggerWcfServiceInfoAttribute)
        };

        public SwaggerSchemaBuilder(Type serviceContractType)
        {
            if (_requiredAttributeTypes.Any(attrType => serviceContractType.GetCustomAttribute(attrType) is null))
            {
                throw new ArgumentException($"{serviceContractType.FullName} does not have required attributes",
                    nameof(serviceContractType));
            }

            _serviceContractType = serviceContractType;
        }

        public SwaggerSchema Build()
        {
            SwaggerSchema swaggerSchema = new SwaggerSchema
            {
                BasePath = _serviceContractType.GetBasePath(),
                Info = _serviceContractType.GetServiceInfo(),
            };

            return swaggerSchema;
        }
    }
}
