using System.ComponentModel;
using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract(Name = "language")]
    public enum Language
    {
        [Description("Undefined language")]
        Unknown = 0,
        English = 1,
        Spanish = 2,
        French = 3,
        Chinese = 4,
    }
}
