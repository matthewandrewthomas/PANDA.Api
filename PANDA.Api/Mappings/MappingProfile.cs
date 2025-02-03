using AutoMapper;
using PANDA.Api.Enums;
using PANDA.Api.Models;
using PANDA.Api.Models.DTOs;

namespace PANDA.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DetermineStatus(src)))
                .ForMember(dest => dest.Clinician, opt => opt.MapFrom(src => src.Clinician.Name))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src.Organisation.Postcode))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ReverseMap()
                .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.Status == AppointmentStatus.Cancelled))
                .ForMember(dest => dest.IsAttended, opt => opt.MapFrom(src => src.Status == AppointmentStatus.Attended))
                .ForMember(dest => dest.PatientEntity, opt => opt.Ignore())
                .ForMember(dest => dest.Clinician, opt => opt.Ignore())
                .ForMember(dest => dest.Organisation, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient));
            CreateMap<Patient, PatientDto>()
                .ReverseMap();
        }

        private AppointmentStatus DetermineStatus(Appointment appointment)
        {
            if (appointment.IsCancelled) return AppointmentStatus.Cancelled;
            if (appointment.IsAttended) return AppointmentStatus.Attended;
            if (appointment.Time > DateTimeOffset.Now) return AppointmentStatus.Active;
            return AppointmentStatus.Missed;
        }
    }
}
