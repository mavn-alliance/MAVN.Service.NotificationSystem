using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate;
using MAVN.Service.NotificationSystem.Controllers;
using MAVN.Service.NotificationSystem.Domain.Models;
using MAVN.Service.NotificationSystem.Domain.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace MAVN.Service.NotificationSystem.Tests
{
    public class NotificationTemplateControllerTest
    {
        [Fact]
        public async Task CreateTemplateAsync_BadRequest_TemplateAndLocal_Exist()
        {
            var templateService = new Mock<ITemplateService>();
            var controller = new NotificationTemplateController(templateService.Object);

            var info = new NotificationTemplateInfo("temp", new List<Localization>
            {
                Localization.From("en")
            });
            templateService.Setup(e => e.GetTemplateInfoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(info));

            var ex = await Record.ExceptionAsync(() =>
                controller.CreateTemplateAsync(new NewTemplateRequest
                {
                    TemplateName = "temp",
                    TemplateBody = "hello",
                    LocalizationCode = "en"
                }));

            Assert.NotNull(ex);
            Assert.IsType<ValidationApiException>(ex);
            Assert.Equal(HttpStatusCode.BadRequest, ((ValidationApiException)ex).StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task UpdateTemplateAsync_SuccessCreate()
        {
            var templateService = new Mock<ITemplateService>();
            var controller = new NotificationTemplateController(templateService.Object);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var info = new NotificationTemplateInfo("temp", new List<Localization>
            {
                Localization.From("en")
            });
            templateService.Setup(e => e.GetTemplateInfoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(info));

            await controller.UpdateTemplateAsync(new NewTemplateRequest { TemplateName = "temp", TemplateBody = "hello", LocalizationCode = "en" });

            templateService.Verify(e => e.CreateOrUpdateTemplateAsync("temp", "hello", Localization.From("en")), Times.Once);

            Assert.Equal((int)HttpStatusCode.NoContent, controller.Response.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task UpdateTemplateAsync_BadRequest_NotFountTemplate()
        {
            var templateService = new Mock<ITemplateService>();
            var controller = new NotificationTemplateController(templateService.Object);

            templateService.Setup(e => e.GetTemplateInfoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((NotificationTemplateInfo)null));

            var ex = await Record.ExceptionAsync(() =>
                controller.UpdateTemplateAsync(new NewTemplateRequest { TemplateName = "temp", TemplateBody = "hello", LocalizationCode = "en-us" }));

            Assert.NotNull(ex);
            Assert.IsType<ValidationApiException>(ex);
            Assert.Equal(HttpStatusCode.BadRequest, ((ValidationApiException)ex).StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task UpdateTemplateAsync_BadRequest_TemplateAndLocal_Exist()
        {
            var templateService = new Mock<ITemplateService>();
            var controller = new NotificationTemplateController(templateService.Object);

            var info = new NotificationTemplateInfo("temp", new List<Localization>
            {
                Localization.From("en")
            });
            templateService.Setup(e => e.GetTemplateInfoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(info));

            var ex = await Record.ExceptionAsync(() =>
                controller.UpdateTemplateAsync(new NewTemplateRequest
                {
                    TemplateName = "temp",
                    TemplateBody = "hello",
                    LocalizationCode = "en-us"
                }));

            Assert.NotNull(ex);
            Assert.IsType<ValidationApiException>(ex);
            Assert.Equal(HttpStatusCode.BadRequest, ((ValidationApiException)ex).StatusCode);

            controller.Dispose();
        }
    }
}
