﻿using System;
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
        public static SwaggerSchema Build(string path)
        {
            return BuildServiceCommon(path, BuildPaths);
        }

        public static SwaggerSchema Build<TBusiness>(string path)
        {
            return BuildServiceCommon(path, BuildPaths<TBusiness>);
        }

        private static SwaggerSchema BuildServiceCommon(string path, Action<SwaggerSchema, IList<string>, List<string>, IList<Type>> buildPaths)
        {
            const string sectionName = "swaggerwcf";
            SwaggerWcfSection config =
                (SwaggerWcfSection)(ConfigurationManager.GetSection(sectionName) ?? new SwaggerWcfSection());

            List<Type> definitionsTypesList = new List<Type>();
            SwaggerSchema service = new SwaggerSchema();
            List<string> hiddenTags = SwaggerWcfEndpoint.FilterHiddenTags(path, GetHiddenTags(config));
            List<string> visibleTags = SwaggerWcfEndpoint.FilterVisibleTags(path, GetVisibleTags(config));
            IReadOnlyDictionary<string, string> settings = GetSettings(config);

            ProcessSettings(service, settings);

            buildPaths(service, hiddenTags, visibleTags, definitionsTypesList);

            service.Definitions = DefinitionsBuilder.Process(hiddenTags, visibleTags, definitionsTypesList);

            return service;
        }


        private static List<string> GetHiddenTags(SwaggerWcfSection config)
        {
            return config.Tags?.OfType<TagElement>()
                       .Where(t => t.Visibile.Equals(false))
                       .Select(t => t.Name)
                       .ToList() ?? new List<string>();
        }

        private static List<string> GetVisibleTags(SwaggerWcfSection config)
        {
            return config.Tags?.OfType<TagElement>()
                       .Where(t => t.Visibile.Equals(true))
                       .Select(t => t.Name)
                       .ToList() ?? new List<string>();
        }

        private static IReadOnlyDictionary<string, string> GetSettings(SwaggerWcfSection config)
        {
            return config.Settings?.OfType<SettingElement>().ToDictionary(se => se.Name, se => se.Value)
                ?? new Dictionary<string, string>();
        }

        private static void ProcessSettings(SwaggerSchema service, IReadOnlyDictionary<string, string> settings)
        {
            if (settings.ContainsKey("BasePath"))
                service.BasePath = settings["BasePath"];
            if (settings.ContainsKey("Host"))
                service.Host = settings["Host"];
            //if (settings.ContainsKey("Schemes"))
            //    service.Schemes = settings["Schemes"].Split(';').ToList();

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

        private static void BuildPaths(SwaggerSchema service, IList<string> hiddenTags, List<string> visibleTags, IList<Type> definitionsTypesList)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            service.Paths = new Dictionary<string, PathItem>();

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

                    IEnumerable<Path> paths = mapper.FindMethods(da.BasePath, ti.AsType(), definitionsTypesList);
                    //service.Paths.AddRange(paths);
                }
            }
        }

        private static void BuildPaths<TBusiness>(SwaggerSchema service, IList<string> hiddenTags, List<string> visibleTags, IList<Type> definitionsTypesList)
        {
            Type type = typeof(TBusiness);
            service.Paths = new Dictionary<string, PathItem>();

            SwaggerWcfAttribute da = type.GetCustomAttribute<SwaggerWcfAttribute>();
            if (da == null || hiddenTags.Any(ht => ht == type.Name))
                return;

            Mapper mapper = new Mapper(hiddenTags, visibleTags);

            IEnumerable<Path> paths = mapper.FindMethods(da.BasePath, type, definitionsTypesList);
            //service.Paths.AddRange(paths);
        }
    }
}
