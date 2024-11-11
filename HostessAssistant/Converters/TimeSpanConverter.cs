using System;
using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ElinaTestProject.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var stringValue = reader.Value.ToString();
            if (TimeSpan.TryParse(stringValue, out var result))
            {
                return result;
            }

            throw new ArgumentException("Failed to convert to TimeSpan.");
        }

        public override void WriteJson(JsonWriter writer, TimeSpan value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
        //public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    var stringValue = reader.GetString();
        //    return TimeSpan.Parse(stringValue.ToString());

        //}

        //public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        //{
        //    writer.WriteStringValue(value.ToString());
        //}
    }
}
