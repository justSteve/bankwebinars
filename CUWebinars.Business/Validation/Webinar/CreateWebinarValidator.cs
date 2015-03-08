using FluentValidation;

namespace CUWebinars.Business.Validation.Webinar
{
    public class CreateWebinarValidator : AbstractValidator<Models.Webinar>
    {
        private const string CannotBeNullOrEmpty = "The {0} cannot be null or empty";
        private const decimal MinDecimal = 0.25M;
        private const string MustBeGreaterThan = "The {0} must be greater than {1}";

        public CreateWebinarValidator()
        {
            // strings
            RuleFor(w => w.Title).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Title");
            RuleFor(w => w.Description).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Description");
            RuleFor(w => w.DescriptionLong).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LongDescription");
            RuleFor(w => w.LearnBody).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LearnBody");
            RuleFor(w => w.LearnCaption).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LearnCaption");
            RuleFor(w => w.WhoAttend).NotEmpty().WithMessage(CannotBeNullOrEmpty, "WhoAttend");
            RuleFor(w => w.RecordingUrl).NotEmpty().WithMessage(CannotBeNullOrEmpty, "RecordingUrl");

            //RuleFor(w => w.Status).Must(x => x == )

            // dates
            RuleFor(w => w.Date).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Date");
            RuleFor(w => w.DateChanged).NotEmpty().WithMessage(CannotBeNullOrEmpty, "DateChanged");
            RuleFor(w => w.DateCreated).NotEmpty().WithMessage(CannotBeNullOrEmpty, "DateCreated");

            // numbers
            RuleFor(w => w.idPresenter).GreaterThan(0).WithMessage(MustBeGreaterThan, "SelectedPresenter", 0);
            RuleFor(w => w.Duration).GreaterThan(d => MinDecimal).WithMessage(MustBeGreaterThan, "Duration", MinDecimal);
            RuleFor(w => (int)w.Status).GreaterThan(0).WithMessage(MustBeGreaterThan, "SelectedStatus", 0);
        }
    }
}
