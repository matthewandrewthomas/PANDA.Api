using FluentValidation;
using PANDA.Api.Models.DTOs;

namespace PANDA.Api.Validators
{
    public class AppointmentDtoValidator : AbstractValidator<AppointmentDto>
    {
        public AppointmentDtoValidator(IValidator<string> nhsNumberValidator)
        {
            RuleFor(appointment => appointment.Patient)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty()
                .SetValidator(nhsNumberValidator);

            RuleFor(patient => patient.Postcode)
                .NotEmpty()
                .Matches(@"^([A-Z]{1,2}[0-9][A-Z0-9]?\s?[0-9][A-Z]{2}|(BF)?[0-9]{1,2}\s?[0-9][A-Z]{2})$")
                .WithMessage("Invalid UK Postcode format.");
        }
    }
}
