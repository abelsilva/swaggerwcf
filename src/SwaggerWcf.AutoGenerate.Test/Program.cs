using Newtonsoft.Json;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;
using SwaggerWcf.Support;
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

            var serviceInfoAttr = serviceContractType.GetCustomAttribute<SwaggerWcfServiceInfoAttribute>()
                ?? throw new Exception($"SwaggerWcfServiceInfoAttribute doesn't exist on ServiceContract {serviceContractType.FullName}");


            Service service = SwaggerWcf.Support.ServiceBuilder.Build("/");
            service.Info = serviceInfoAttr.AssignServiceInfoValuesTo(new Info());

            var contactInfoAttr = serviceContractType.GetCustomAttribute<SwaggerWcfContactInfoAttribute>();
            if (contactInfoAttr != null)
            {
                service.Info.Contact = (InfoContact)contactInfoAttr;
            }

            var licenseInfoAttr = serviceContractType.GetCustomAttribute<SwaggerWcfLicenseInfoAttribute>();
            if (licenseInfoAttr != null)
            {
                service.Info.License = (InfoLicense)licenseInfoAttr;
            }

            string json = JsonConvert.SerializeObject(service, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        }
    }
}
