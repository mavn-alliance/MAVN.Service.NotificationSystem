using System.Text.RegularExpressions;
using FluentValidation;
using MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate;

namespace MAVN.Service.NotificationSystem.Validation
{
    public class NewTemplateRequestValidator : AbstractValidator<NewTemplateRequest>
    {
        public NewTemplateRequestValidator()
        {
            RuleFor(x => x.TemplateName)
                .NotEmpty()
                .WithMessage("Template Name is required")
                .MinimumLength(3)
                .WithMessage("Template Name cannot be less then 3 characters in length")
                .MaximumLength(63)
                .WithMessage("Template Name cannot be more then 63 characters in length")
                .Custom((templateName, context) =>
                {
                    if (!Regex.IsMatch(templateName, "^(?!-)(?!.*--)[a-z0-9\\d-]+(?<!-)$"))
                        context.AddFailure(
                            "Template Name can only contain lowercase alphanumeric characters and hyphen (except as the first or the last character)");
                });

            RuleFor(x => x.LocalizationCode)
                .NotEmpty()
                .WithMessage("Template Localization Code is required")
                .Custom((templateLocalizationCode, context) =>
                {
                    if (!Regex.IsMatch(templateLocalizationCode, "^(?!-)(?!.*--)[a-zA-Z\\d-]+(?<!-)$"))
                        context.AddFailure(
                            "Template Localization Code can only lower and uppercase alphabet characters and hyphen (except as the first or the last character)");
                });
        }
    }
}
