using FluentValidation;
using Lykke.Service.NotificationSystem.Client.Models.Message;

namespace Lykke.Service.NotificationSystem.Validation
{
    public class SensSmsRequestValidator : AbstractValidator<SendSmsRequest>
    {
        public SensSmsRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer id is required");

            RuleFor(x => x.MessageTemplateId)
                .NotEmpty()
                .WithMessage("Message template id is required");

            RuleFor(x => x.Source)
                .NotEmpty()
                .WithMessage("Source is required");
        }
    }
}
