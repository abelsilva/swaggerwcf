using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Property
    {
        public Property()
        {
            Maximum = decimal.MaxValue;
            Minimum = decimal.MinValue;
            MaxLength = int.MaxValue;
            MinLength = int.MinValue;
            MaxItems = int.MaxValue;
            MinItems = int.MinValue;
            MultipleOf = decimal.MinValue;
        }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool Required { get; set; }

        public TypeFormat TypeFormat { get; set; }

        public bool AllowEmptyValue { get; set; }

        public ParameterItems Items { get; set; }

        public CollectionFormat CollectionFormat { get; set; }

        public string Default { get; set; }

        public decimal Maximum { get; set; }

        public bool ExclusiveMaximum { get; set; }

        public decimal Minimum { get; set; }

        public bool ExclusiveMinimum { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        public string Pattern { get; set; }

        public int MaxItems { get; set; }

        public int MinItems { get; set; }

        public bool UniqueItems { get; set; }

        public List<int> Enum { get; set; }

        public decimal MultipleOf { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Title))
            {
                writer.WritePropertyName("title");
                writer.WriteValue(Title);
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            writer.WritePropertyName("required");
            writer.WriteValue(Required);
            if (TypeFormat.Type != ParameterType.Unknown)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(TypeFormat.Type.ToString().ToLower());
                if (!string.IsNullOrWhiteSpace(TypeFormat.Format))
                {
                    writer.WritePropertyName("format");
                    writer.WriteValue(TypeFormat.Format);
                }
            }
            writer.WritePropertyName("allowEmptyValue");
            writer.WriteValue(AllowEmptyValue);
            if (TypeFormat.Type == ParameterType.Array && Items != null)
            {
                writer.WritePropertyName("items");
                Items.Serialize(writer);
            }
            if (TypeFormat.Type == ParameterType.Array)
            {
                writer.WritePropertyName("collectionFormat");
                writer.WriteValue(CollectionFormat);
            }
            if (!string.IsNullOrWhiteSpace(Default))
            {
                writer.WritePropertyName("default");
                writer.WriteValue(Default);
            }
            if (Maximum != decimal.MaxValue)
            {
                writer.WritePropertyName("maximum");
                writer.WriteValue(Maximum);
                writer.WritePropertyName("exclusiveMaximum");
                writer.WriteValue(ExclusiveMaximum);
            }
            if (Minimum != decimal.MinValue)
            {
                writer.WritePropertyName("minimum");
                writer.WriteValue(Minimum);
                writer.WritePropertyName("exclusiveMinimum");
                writer.WriteValue(ExclusiveMinimum);
            }
            if (MaxLength != int.MaxValue)
            {
                writer.WritePropertyName("maxLength");
                writer.WriteValue(MaxLength);
            }
            if (MinLength != int.MinValue)
            {
                writer.WritePropertyName("minLength");
                writer.WriteValue(MinLength);
            }
            if (!string.IsNullOrWhiteSpace(Pattern))
            {
                writer.WritePropertyName("pattern");
                writer.WriteValue(Pattern);
            }
            if (MaxItems != int.MaxValue)
            {
                writer.WritePropertyName("maxItems");
                writer.WriteValue(MaxItems);
            }
            if (MinItems != int.MinValue)
            {
                writer.WritePropertyName("minItems");
                writer.WriteValue(MinItems);
            }
            writer.WritePropertyName("uniqueItems");
            writer.WriteValue(UniqueItems);
            if (Enum != null && Enum.Any())
            {
                writer.WritePropertyName("enum");
                writer.WriteStartArray();
                foreach (int e in Enum)
                {
                    writer.WriteValue(e);
                }
                writer.WriteEndArray();
            }
            if (MultipleOf != decimal.MinValue)
            {
                writer.WritePropertyName("multipleOf");
                writer.WriteValue(MultipleOf);
            }
            
            writer.WriteEndObject();
        }
    }
}