using Microsoft.EntityFrameworkCore;
using PANDA.Api.Data;
using PANDA.Api.Models.DTOs;
using PANDA.Api.Models;
using Microsoft.Extensions.Localization;
using AutoMapper;

namespace PANDA.Api.Services
{
    public class PatientService : IPatientService
    {
        private readonly PandaContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<PatientService> _localizer;

        public PatientService(PandaContext context, IMapper mapper, IStringLocalizer<PatientService> localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsAsync()
        {
            var patients = await _context.Patients.ToListAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(string nhsNumber)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.NhsNumber == nhsNumber);
            if (patient == null)
            {
                return null;
            }

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> AddPatientAsync(PatientDto patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patientDto;
        }

        public async Task UpdatePatientAsync(PatientDto patientDto)
        {
            var patient = await _context.Patients.FindAsync(patientDto.NhsNumber);
            if (patient == null)
            {
                throw new Exception(_localizer["PatientNotFound"]);
            }

            _mapper.Map(patientDto, patient);

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePatientAsync(string nhsNumber)
        {
            var patient = await _context.Patients.FindAsync(nhsNumber);
            if (patient == null)
            {
                return false;
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PatientExistsAsync(string nhsNumber)
        {
            return await _context.Patients.AnyAsync(e => e.NhsNumber == nhsNumber);
        }
    }
}
