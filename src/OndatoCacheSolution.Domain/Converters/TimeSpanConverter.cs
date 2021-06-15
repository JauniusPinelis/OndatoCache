using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OndatoCacheSolution.Domain.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Parse(reader.GetString());
        }

        public TimeSpan Parse(string input)
        {
            return TimeSpan.Parse(input);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
