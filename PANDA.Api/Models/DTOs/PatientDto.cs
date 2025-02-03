using System.Text.Json.Serialization;

namespace PANDA.Api.Models.DTOs
{
    public class PatientDto
    {
        [JsonPropertyName("nhs_number")]
        public string NhsNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
    }
}
