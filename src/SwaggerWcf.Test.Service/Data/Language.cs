using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract(Name = "language")]
    public enum Language
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,
        [EnumMember(Value = "english")]
        English = 1,
        [EnumMember(Value = "spanish")]
        Spanish = 2,
        [EnumMember(Value = "french")]
        French = 3,
        [EnumMember(Value = "chinese")]
        Chinese = 4,
    }
}
