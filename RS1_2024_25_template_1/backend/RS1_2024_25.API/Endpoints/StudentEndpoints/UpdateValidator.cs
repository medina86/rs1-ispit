using FluentValidation;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    public class UpdateValidator: AbstractValidator<StudentUpdateRequest>
    {
        public UpdateValidator() { 
            RuleFor(x=>x.Phone).NotEmpty().Matches("^06\\d-\\d\\d\\d-\\d\\d\\d$").WithMessage("Format 06x-xxx-xxx");
        }
    }
}
