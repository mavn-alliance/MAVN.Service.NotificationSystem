using FluentValidation.TestHelper;
using MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate;
using MAVN.Service.NotificationSystem.Validation;
using Xunit;

namespace MAVN.Service.NotificationSystem.Tests.Validation
{
    public class NewTemplateRequestValidatorTests
    {
        private const string TemplateNameErrorMessage =
            "Template Name can only contain lowercase alphanumeric characters and hyphen (except as the first or the last character)";

        private const string LocalizationCodeErrorMessage =
            "Template Localization Code can only lower and uppercase alphabet characters and hyphen (except as the first or the last character)";

        private readonly NewTemplateRequestValidator _validator;

        public NewTemplateRequestValidatorTests()
        {
            _validator = new NewTemplateRequestValidator();
        }

        [Fact]
        public void When_TemplateName_Contains_Uppercase_Character_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "Thisisnotallowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.TemplateName, template);

            result.WithErrorMessage(TemplateNameErrorMessage);
        }

        [Fact]
        public void When_TemplateName_Contains_Consecutive_Hyphen_Characters_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "Thisisnot--allowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.TemplateName, template);

            result.WithErrorMessage(TemplateNameErrorMessage);
        }

        [Fact]
        public void When_TemplateName_Starts_With_Hyphen_Character_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "-thisisnotallowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.TemplateName, template);

            result.WithErrorMessage(TemplateNameErrorMessage);
        }

        [Fact]
        public void When_TemplateName_Ends_With_Hyphen_Character_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "thisisnotallowed-",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.TemplateName, template);

            result.WithErrorMessage(TemplateNameErrorMessage);
        }

        [Fact]
        public void When_LocalizationCode_Contains_Not_Allowed_Characters_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "this-is--allowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us UD"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.LocalizationCode, template);

            result.WithErrorMessage(LocalizationCodeErrorMessage);
        }

        [Fact]
        public void When_LocalizationCode_Contains_Consecutive_Hyphen_Characters_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "thisisallowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en--us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.LocalizationCode, template);

            result.WithErrorMessage(LocalizationCodeErrorMessage);
        }

        [Fact]
        public void When_LocalizationCode_Starts_With_Hyphen_Character_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "thisisallowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "-en-us"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.LocalizationCode, template);

            result.WithErrorMessage(LocalizationCodeErrorMessage);
        }

        [Fact]
        public void When_LocalizationCode_Ends_With_Hyphen_Character_Expect_An_Error()
        {
            var template = new NewTemplateRequest
            {
                TemplateName = "thisisallowed",
                TemplateBody = "Template Body Example",
                LocalizationCode = "en-us-"
            };

            var result = _validator.ShouldHaveValidationErrorFor(x => x.LocalizationCode, template);

            result.WithErrorMessage(LocalizationCodeErrorMessage);
        }
    }
}
