﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SwaggerWcf.Models
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum ParameterType
    {
        Unknown = 0,
        Void = 1,
        String = 2,
        Number = 3,
        Integer = 4,
        Boolean = 5,
        Array = 6,
        File = 7,
        Object = 8
    }
}