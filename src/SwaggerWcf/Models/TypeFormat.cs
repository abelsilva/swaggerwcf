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

        internal bool IsPrimitiveType
        {
            get
            {
                return Type == ParameterType.Boolean ||
                       Type == ParameterType.Integer ||
                       Type == ParameterType.Number ||
                       (Type == ParameterType.String && string.IsNullOrEmpty(Format));
            }
        }
    }
}
