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
            var attr = _serviceContractType.GetCustomAttribute<SwaggerWcfAttribute>();

            SwaggerSchema swaggerSchema = new SwaggerSchema
            {
                Info = _serviceContractType.GetServiceInfo(),
                Host = attr.Host,
                BasePath = attr.BasePath,
                Schemes = attr.Schemes,
                Consumes = attr.Consumes,
                Produces = attr.Produces,
            };

            return swaggerSchema;
        }
    }
}
