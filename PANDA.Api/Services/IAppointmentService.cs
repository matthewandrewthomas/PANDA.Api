using PANDA.Api.Models.DTOs;

namespace PANDA.Api.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByNhsNumberAsync(string nhsNumber);
        Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id);
        Task AddAppointmentAsync(AppointmentDto appointmentDto);
        Task UpdateAppointmentAsync(AppointmentDto appointmentDto);
    }
}
