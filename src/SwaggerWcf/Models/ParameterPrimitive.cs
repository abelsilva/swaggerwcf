using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class ParameterPrimitive : ParameterBase
    {
        public ParameterPrimitive()
        {
            Maximum = decimal.MaxValue;
            Minimum = decimal.MinValue;
            MaxLength = int.MaxValue;
            MinLength = int.MinValue;
            MaxItems = int.MaxValue;
            MinItems = int.MinValue;
            MultipleOf = decimal.MinValue;
        }

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

        public override void Serialize(JsonWriter writer)
        {
            Serialize(writer, false);
        }
        public override void Serialize(JsonWriter writer, bool skipStartEndObject)
        {
            if (!skipStartEndObject)
                writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }
            if (In != InType.Unknown)
            {
                writer.WritePropertyName("in");
                writer.WriteValue(In.ToString().ToLower());
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            if (Required)
            {
                writer.WritePropertyName("required");
                writer.WriteValue(Required);
            }
            if (TypeFormat.Type != ParameterType.Unknown)
            {
                if (In == InType.Body)
                {
                    writer.WritePropertyName("schema");
                    writer.WriteStartObject();

                    writer.WritePropertyName("type");
                    writer.WriteValue(TypeFormat.Type.ToString().ToLower());

                    if (TypeFormat.Type == ParameterType.Array && Items != null)
                    {
                        writer.WritePropertyName("items");

                        writer.WriteStartObject();

                        writer.WritePropertyName("$ref");
                        writer.WriteValue($"#/definitions/{Items?.Ref ?? (Items?.Items as ParameterSchema)?.SchemaRef}");
                        //writer.WriteValue($"#/definitions/{Items?.Ref}");

                        writer.WriteEndObject();
                    }

                    if (!string.IsNullOrWhiteSpace(TypeFormat.Format))
                    {
                        writer.WritePropertyName("format");
                        writer.WriteValue(TypeFormat.Format);
                    }

                    writer.WriteEndObject();
                }
                else
                {
                    writer.WritePropertyName("type");
                    writer.WriteValue(TypeFormat.Type.ToString().ToLower());
                    if (!string.IsNullOrWhiteSpace(TypeFormat.Format))
                    {
                        writer.WritePropertyName("format");
                        writer.WriteValue(TypeFormat.Format);
                    }
                }
            }
            if (AllowEmptyValue)
            {
                writer.WritePropertyName("allowEmptyValue");
                writer.WriteValue(AllowEmptyValue);
            }
            if (In != InType.Body && TypeFormat.Type == ParameterType.Array && Items != null)
            {
                writer.WritePropertyName("items");
                Items.Serialize(writer);
            }
            if (In != InType.Body && TypeFormat.Type == ParameterType.Array && Items != null)
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
            if (UniqueItems)
            {
                writer.WritePropertyName("uniqueItems");
                writer.WriteValue(UniqueItems);
            }
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

            if (!skipStartEndObject)
                writer.WriteEndObject();
        }
    }
}
