using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
using FluentValidation;

namespace CUWebinars.Business.Validation.Webinar
{
    public class CreateWebinarValidator : AbstractValidator<Models.Webinar>
    {
        protected const string CannotBeNullOrEmpty = "The {0} cannot be null or empty";
        protected const decimal MinDecimal = 0.25M;
        protected const string MustBeGreaterThan = "The {0} must be greater than {1}";


        public CreateWebinarValidator()
        {
            RuleFor(w => w.Title).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Title");
            RuleFor(w => w.Description).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Description");
            RuleFor(w => w.DescriptionLong).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LongDescription");
            RuleFor(w => w.LearnBody).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LearnBody");
            RuleFor(w => w.LearnCaption).NotEmpty().WithMessage(CannotBeNullOrEmpty, "LearnCaption");
            RuleFor(w => w.WhoAttend).NotEmpty().WithMessage(CannotBeNullOrEmpty, "WhoAttend");

            // dates
            RuleFor(w => w.Date).NotEmpty().WithMessage(CannotBeNullOrEmpty, "Date");
            RuleFor(w => w.DateChanged).NotEmpty().WithMessage(CannotBeNullOrEmpty, "DateChanged");
            RuleFor(w => w.DateCreated).NotEmpty().WithMessage(CannotBeNullOrEmpty, "DateCreated");

            // numbers
            RuleFor(w => w.idPresenter).GreaterThan(0).WithMessage(MustBeGreaterThan, "SelectedPresenter", 0);
            RuleFor(w => w.Duration).GreaterThan(d => MinDecimal).WithMessage(MustBeGreaterThan, "Duration", MinDecimal);
            RuleFor(w => (int)w.Status).GreaterThan(0).WithMessage(MustBeGreaterThan, "SelectedStatus", 0);

            // items which are lookups
            RuleFor(w => w.RegTypesGroupsXref).Must(MustHaveAtLeastOneRegTypeGroup).WithMessage("Webinar must have at least 1 RegType");
            RuleFor(w => w.WebinarTopicXrefs).Must(MustHaveAtLeastOneTopic).WithMessage("Webinar must have at least 1 topic");
        }

        protected virtual bool MustHaveAtLeastOneRegTypeGroup(ICollection<RegTypesGroupsXref> regTypesGroupsXrefs)
        {
            return regTypesGroupsXrefs.Any();
        }

        protected virtual bool MustHaveAtLeastOneTopic(ICollection<WebinarTopicXref> webinarTopicXrefs)
        {
            return webinarTopicXrefs.Any();
        }

    }
}
