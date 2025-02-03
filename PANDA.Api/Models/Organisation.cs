using System.ComponentModel.DataAnnotations;

namespace PANDA.Api.Models
{
    public class Organisation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? OrgCode { get; set; } // Optional (for now...)

        public string? Name { get; set; } // Optional (for now...)

        public string Postcode { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
