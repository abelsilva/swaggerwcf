using System.ComponentModel;
using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract(Name = "language")]
    public enum Language
    {
        [Description("language is not defined")]
        Unknown = 0,
        English = 1,
        Spanish = 2,
        French = 3,
        Chinese = 4,
    }
}
