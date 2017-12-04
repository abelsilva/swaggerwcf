﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public struct TypeFormat
    {
        public ParameterType Type;

        public string Format;

        public TypeFormat(ParameterType type, string format)
        {
            Type = type;
            Format = format;
        }

        internal bool IsPrimitiveType => Type == ParameterType.Boolean ||
                                         Type == ParameterType.Integer ||
                                         Type == ParameterType.Number ||
                                         Type == ParameterType.String && !string.Equals(Format, "stream");

        // possible that enum should be included in primitive type?
        internal bool IsEnum => Type == ParameterType.String && Format == "enum";
    }
}