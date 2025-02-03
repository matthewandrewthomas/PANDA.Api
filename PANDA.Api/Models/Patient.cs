using System.ComponentModel.DataAnnotations;

namespace PANDA.Api.Models
{
    public class Patient
    {
        [Key]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression("^[0-9]{10}$")]
        public string NhsNumber { get; set; }

        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Postcode { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
