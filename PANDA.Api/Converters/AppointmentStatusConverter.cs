using PANDA.Api.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PANDA.Api.Converters
{
    public class AppointmentStatusConverter : JsonConverter<AppointmentStatus>
    {
        public override AppointmentStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (value == null)
            {
                throw new JsonException("Value is null");
            }

            foreach (var enumValue in Enum.GetValues<AppointmentStatus>())
            {
                if (string.Equals(enumValue.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new JsonException($"Unable to convert '{value}' to AppointmentStatus enum.");
        }

        public override void Write(Utf8JsonWriter writer, AppointmentStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
