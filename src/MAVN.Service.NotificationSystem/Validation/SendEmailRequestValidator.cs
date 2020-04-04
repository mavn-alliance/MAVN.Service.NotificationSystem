using FluentValidation;
using MAVN.Service.NotificationSystem.Client.Models.Message;

namespace MAVN.Service.NotificationSystem.Validation
{
    public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
    {
        public SendEmailRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer id is required");

            RuleFor(x => x.SubjectTemplateId)
                .NotEmpty()
                .WithMessage("Subject template id is required");

            RuleFor(x => x.MessageTemplateId)
                .NotEmpty()
                .WithMessage("Message template id is required");

            RuleFor(x => x.Source)
                .NotEmpty()
                .WithMessage("Source is required");
        }
    }
}
