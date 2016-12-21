using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SwaggerWcf.Attributes;
using SwaggerWcf.Configuration;
using SwaggerWcf.Models;
using SettingElement = SwaggerWcf.Configuration.SettingElement;

namespace SwaggerWcf.Support
{
    internal class ServiceBuilder
    {
        public static Service Build()
        {
            Service service = BuildService();

            return service;
        }

        private static Service BuildService()
        {
            const string sectionName = "swaggerwcf";
            SwaggerWcfSection config =
                (SwaggerWcfSection)(ConfigurationManager.GetSection(sectionName) ?? new SwaggerWcfSection());
            
            List<Type> definitionsTypesList = new List<Type>();
            Service service = new Service();
            List<string> hiddenTags = GetHiddenTags(config);
            List<string> visibleTags = GetVisibleTags(config);
            IReadOnlyDictionary<string, string> settings = GetSettings(config);

            ProcessSettings(service, settings);

            BuildPaths(service, hiddenTags, visibleTags, definitionsTypesList);

            service.Definitions = DefinitionsBuilder.Process(hiddenTags, visibleTags, definitionsTypesList);

            return service;
        }

        private static List<string> GetHiddenTags(SwaggerWcfSection config)
        {
            return config.Tags == null
                       ? new List<string>()
                       : config.Tags.OfType<TagElement>()
                               .Where(t => t.Visibile.Equals(false))
                               .Select(t => t.Name)
                               .ToList();
        }

        private static List<string> GetVisibleTags(SwaggerWcfSection config)
        {
            return config.Tags == null
                       ? new List<string>()
                       : config.Tags.OfType<TagElement>()
                               .Where(t => t.Visibile.Equals(true))
                               .Select(t => t.Name)
                               .ToList();
        }

        private static IReadOnlyDictionary<string, string> GetSettings(SwaggerWcfSection config)
        {
            return config.Settings == null
                       ? new Dictionary<string, string>()
                       : config.Settings.OfType<SettingElement>().ToDictionary(se => se.Name, se => se.Value);
        }

        private static void ProcessSettings(Service service, IReadOnlyDictionary<string, string> settings)
        {
            if (settings.ContainsKey("BasePath"))
                service.BasePath = settings["BasePath"];
            if (settings.ContainsKey("Host"))
                service.Host = settings["Host"];

            if (settings.Keys.Any(k => k.StartsWith("Info")))
                service.Info = new Info();
            if (settings.ContainsKey("InfoDescription"))
                service.Info.Description = settings["InfoDescription"];
            if (settings.ContainsKey("InfoVersion"))
                service.Info.Version = settings["InfoVersion"];
            if (settings.ContainsKey("InfoTermsOfService"))
                service.Info.TermsOfService = settings["InfoTermsOfService"];
            if (settings.ContainsKey("InfoTitle"))
                service.Info.Title = settings["InfoTitle"];

            if (settings.Keys.Any(k => k.StartsWith("InfoContact")))
                service.Info.Contact = new InfoContact();
            if (settings.ContainsKey("InfoContactName"))
                service.Info.Contact.Name = settings["InfoContactName"];
            if (settings.ContainsKey("InfoContactUrl"))
                service.Info.Contact.Url = settings["InfoContactUrl"];
            if (settings.ContainsKey("InfoContactEmail"))
                service.Info.Contact.Email = settings["InfoContactEmail"];

            if (settings.Keys.Any(k => k.StartsWith("InfoLicense")))
                service.Info.License = new InfoLicense();
            if (settings.ContainsKey("InfoLicenseUrl"))
                service.Info.License.Url = settings["InfoLicenseUrl"];
            if (settings.ContainsKey("InfoLicenseName"))
                service.Info.License.Name = settings["InfoLicenseName"];
        }

        private static void BuildPaths(Service service, IList<string> hiddenTags, List<string> visibleTags, IList<Type> definitionsTypesList)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            service.Paths = new List<Path>();

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<TypeInfo> types;
                try
                {
                    types = assembly.DefinedTypes;
                }
                catch (Exception)
                {
                    // ignore assembly and continue
                    continue;
                }

                foreach (TypeInfo ti in types)
                {
                    SwaggerWcfAttribute da = ti.GetCustomAttribute<SwaggerWcfAttribute>();
                    if (da == null || hiddenTags.Any(ht => ht == ti.AsType().Name))
                        continue;

                    Mapper mapper = new Mapper(hiddenTags, visibleTags);

                    IEnumerable<Path> paths = mapper.FindMethods(da.ServicePath, ti.AsType(), definitionsTypesList);
                    service.Paths.AddRange(paths);
                }
            }
        }
    }
}
