using System.ComponentModel.DataAnnotations;

namespace PANDA.Api.Models
{
    public class Department
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Appointment>? MissedAppointments
        {
            get
            {
                return Appointments?.Where(a => !a.IsAttended && !a.IsCancelled && a.Time < DateTimeOffset.Now).ToList();
            }
        }
    }
}