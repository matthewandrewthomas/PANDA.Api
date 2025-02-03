using PANDA.Api.Converters;
using PANDA.Api.Enums;
using System.Text.Json.Serialization;

namespace PANDA.Api.Models.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Patient { get; set; } // NHS Number
        public string Duration { get; set; }
        public string Clinician { get; set; }
        public string Department { get; set; }
        public string Postcode { get; set; }

        [JsonConverter(typeof(AppointmentStatusConverter))]
        public AppointmentStatus Status { get; set; }
    }
}
