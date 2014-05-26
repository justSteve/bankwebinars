using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using FluentValidation;

namespace CUWebinars.Business.Validation.Order
{
    public class CreateOrderValidator : AbstractValidator<Models.Order>
    {
        private readonly IWebinarRepository _webinarRepository;

        public CreateOrderValidator(IWebinarRepository webinarRepository)
        {
            _webinarRepository = webinarRepository;

            RuleFor(o => o.idUser).GreaterThanOrEqualTo(1).WithMessage("The WebUserId was less than 1.");

            RuleFor(o => o.idUser)
                .Must(UserHasExistingWebinarInNonCancelledState)
                .WithMessage("The WebUser already has at least 1 order which is in a non-cancelled state");
        }

        private bool UserHasExistingWebinarInNonCancelledState(Models.Order order, int userId)
        {
            var webinarId = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Webinar.idWebinar;

            var webinar = _webinarRepository
                .GetAllOrdersByWebinarForUser(webinarId, userId)
                .ToList()
                .Where(o => o.OrderStatus == OrderStatus.InProcess 
                    || o.OrderStatus == OrderStatus.Submitted 
                    || o.OrderStatus == OrderStatus.Billed
                    || o.OrderStatus == OrderStatus.Paid
                    || o.OrderStatus == OrderStatus.AwaitingVerification
                    );
            
            return !webinar.Any();
        }
    }
}
