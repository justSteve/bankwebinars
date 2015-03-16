using FluentValidation;

namespace CUWebinars.Business.Validation.Webinar
{
    public class UpdateWebinarValidator : CreateWebinarValidator
    {
        public UpdateWebinarValidator()
        {
            RuleFor(w => w.RecordingUrl).NotEmpty().WithMessage(CannotBeNullOrEmpty, "RecordingUrl");
        }
    }
}
