namespace SwaggerWcf.Models
{
    public struct TypeFormat
    {
        public ParameterType Type;

        public string Format;

        public TypeFormat(ParameterType type, string format)
        {
            Type = type;
            Format = format;
        }
    }
}
