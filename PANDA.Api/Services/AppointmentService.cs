using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using PANDA.Api.Data;
using PANDA.Api.Enums;
using PANDA.Api.Models;
using PANDA.Api.Models.DTOs;

namespace PANDA.Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly PandaContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AppointmentService> _localizer;

        public AppointmentService(PandaContext context, IMapper mapper, IStringLocalizer<AppointmentService> localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByNhsNumberAsync(string nhsNumber)
        {
            var appointments = await _context.Appointments
                                             .Where(a => a.Patient == nhsNumber)
                                             .Include(a => a.Clinician)
                                             .Include(a => a.Organisation)
                                             .Include(a => a.Department)
                                             .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _context.Appointments
                                            .Include(a => a.Clinician)
                                            .Include(a => a.Organisation)
                                             .Include(a => a.Department)
                                            .FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null)
            {
                return null;
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task AddAppointmentAsync(AppointmentDto appointmentDto)
        {
            var patient = await _context.Patients.FindAsync(appointmentDto.Patient);
            if (patient == null)
            {
                throw new Exception(_localizer["PatientNotFound"]);
            }

            var clinician = await GetOrCreateClinicianAsync(appointmentDto.Clinician);
            var organisation = await GetOrCreateOrganisationAsync(appointmentDto.Postcode);
            var department = await GetOrCreateDepartmentAsync(appointmentDto.Department);

            ValidateAppointmentStatus(appointmentDto);

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            appointment.PatientEntity = patient;
            appointment.ClinicianId = clinician.Id;
            appointment.OrganisationId = organisation.Id;
            appointment.DepartmentId = department.Id;

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(AppointmentDto appointmentDto)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentDto.Id);
            if (appointment == null)
            {
                throw new Exception(_localizer["AppointmentNotFound"]);
            }

            var patient = await _context.Patients.FindAsync(appointmentDto.Patient);
            if (patient == null)
            {
                throw new Exception(_localizer["PatientNotFound"]);
            }

            var clinician = await GetOrCreateClinicianAsync(appointmentDto.Clinician);
            var organisation = await GetOrCreateOrganisationAsync(appointmentDto.Postcode);
            var department = await GetOrCreateDepartmentAsync(appointmentDto.Department);

            ValidateAppointmentStatus(appointmentDto);

            if (appointment.IsCancelled && appointmentDto.Status != AppointmentStatus.Cancelled)
            {
                throw new Exception(_localizer["CannotUpdateCancelledAppointment"]);
            }

            _mapper.Map(appointmentDto, appointment);
            appointment.PatientEntity = patient;
            appointment.ClinicianId = clinician.Id;
            appointment.OrganisationId = organisation.Id;
            appointment.DepartmentId = department.Id;

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        private async Task<Clinician> GetOrCreateClinicianAsync(string clinicianName)
        {
            var clinician = await _context.Clinicians.FirstOrDefaultAsync(c => c.Name == clinicianName);
            if (clinician == null)
            {
                clinician = new Clinician { Name = clinicianName };
                await _context.Clinicians.AddAsync(clinician);
                await _context.SaveChangesAsync();
            }
            return clinician;
        }

        private async Task<Organisation> GetOrCreateOrganisationAsync(string postcode)
        {
            var organisation = await _context.Organisations.FirstOrDefaultAsync(o => o.Postcode == postcode);
            if (organisation == null)
            {
                organisation = new Organisation { Postcode = postcode };
                await _context.Organisations.AddAsync(organisation);
                await _context.SaveChangesAsync();
            }
            return organisation;
        }

        private async Task<Department> GetOrCreateDepartmentAsync(string departmentName)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(o => o.Name == departmentName);
            if (department == null)
            {
                department = new Department { Name = departmentName };
                await _context.Departments.AddAsync(department);
                await _context.SaveChangesAsync();
            }
            return department;
        }

        private void ValidateAppointmentStatus(AppointmentDto appointmentDto)
        {
            if ((appointmentDto.Status == AppointmentStatus.Missed && appointmentDto.Time > DateTimeOffset.Now) ||
                (appointmentDto.Status == AppointmentStatus.Active && appointmentDto.Time <= DateTimeOffset.Now))
            {
                throw new Exception(_localizer["InvalidAppointmentStatus"]);
            }
        }
    }
}
