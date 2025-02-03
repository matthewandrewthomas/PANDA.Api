using System.ComponentModel.DataAnnotations;

namespace PANDA.Api.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTimeOffset Time { get; set; }

        public string Duration { get; set; }

        public Guid ClinicianId { get; set; }

        public Clinician Clinician { get; set; }

        public Guid DepartmentId { get; set; }

        public Department Department { get; set; }

        public Guid OrganisationId { get; set; }

        public Organisation Organisation { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [RegularExpression("^[0-9]{10}$")]
        public string Patient { get; set; }

        public Patient PatientEntity { get; set; }

        public bool IsCancelled { get; set; }

        public bool IsAttended { get; set; }
    }
}
