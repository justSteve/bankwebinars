using CUWebinars.Business.Constants;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace CUWebinars.Business.Validation.Identity
{
    public class PostEventMaterialsAccessClaimValidator : AbstractValidator<Tuple<string,string>>
    {
        public PostEventMaterialsAccessClaimValidator()
        {
            RuleFor(t => t.Item2).Must(ValueMustConformToStructureForThisClaim)
                .WithMessage("PostEventMaterialsAccessClaim needs to be in the following format: OrderId:yyyy-mm-dd");
        }

        private bool ValueMustConformToStructureForThisClaim(Tuple<string,string> claimAndValue, string claimValue)
        {
            if (claimAndValue.Item1.Equals(ClaimTypes.DisplayPostEventMaterials, StringComparison.OrdinalIgnoreCase))
            {
                var regex = new Regex(@"^\d+[:]\d{4}[-]\d{1,2}[-]\d{1,2}$");

                if (regex.Matches(claimValue).Count > 0)
                {
                    return true;
                }
                return false;
            }

            return true;
        }
    }
}
