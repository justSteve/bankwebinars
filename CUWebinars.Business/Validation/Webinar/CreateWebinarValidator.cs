using FluentValidation;

namespace CUWebinars.Business.Validation.Webinar
{
    public class CreateWebinarValidator : UpdateWebinarValidator
    {
        public CreateWebinarValidator()
        {
            RuleFor(w => w.RecordingUrl).NotEmpty().WithMessage(CannotBeNullOrEmpty, "RecordingUrl");
        }
    }
}
