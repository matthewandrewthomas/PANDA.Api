using PANDA.Api.Models.DTOs;

namespace PANDA.Api.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(string nhsNumber);
        Task<PatientDto> AddPatientAsync(PatientDto patientDto);
        Task UpdatePatientAsync(PatientDto patientDto);
        Task<bool> DeletePatientAsync(string nhsNumber);
        Task<bool> PatientExistsAsync(string nhsNumber);
    }
}
