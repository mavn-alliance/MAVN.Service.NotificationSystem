using System.Linq;
using Lykke.Service.NotificationSystem.Domain.Models;
using Xunit;

namespace Lykke.Service.NotificationSystem.Tests
{
    public class NotificationTemplateContentTest
    {
        [Fact]
        public void CreateTemplate_SimpleText()
        {
            var template = new NotificationTemplateContent("test", Localization.From("ru"), "Hello world");

            Assert.Equal("test", template.Name);
            Assert.Equal("ru", template.Localization.ToString());
            Assert.Equal("Hello world", template.Content);
            Assert.Equal(0, template.Keys.Count);
        }

        [Fact]
        public void CreateTemplate_PlaceHolders()
        {
            var text = "Hello ${PD::name}. Your code ${code}";
            var template = new NotificationTemplateContent("test", Localization.From("ru"), text);

            Assert.Equal("test", template.Name);
            Assert.Equal("ru", template.Localization.ToString());
            Assert.Equal(text, template.Content);
            Assert.Equal(2, template.Keys.Count);

            var key = template.Keys.FirstOrDefault(k => k.Key == "name");
            Assert.NotNull(key);
            Assert.Equal("name", key.Key);
            Assert.Equal("PD", key.Namespace);

            key = template.Keys.FirstOrDefault(k => k.Key == "code");
            Assert.NotNull(key);
            Assert.Equal("code", key.Key);
            Assert.Equal(string.Empty, key.Namespace);
        }

        [Fact]
        public void CreateTemplate_UpdateContent()
        {
            var template = new NotificationTemplateContent("test", Localization.From("ru"), "Hello world");
            Assert.Equal("Hello world", template.Content);
            Assert.Equal(0, template.Keys.Count);

            var text = "Hello ${PD::name}. Your code ${code}. Thank you ${PD::name}";
            template.SetContent(text);

            Assert.Equal("test", template.Name);
            Assert.Equal("ru", template.Localization.ToString());
            Assert.Equal(text, template.Content);
            Assert.Equal(2, template.Keys.Count);

            var key = template.Keys.FirstOrDefault(k => k.Key == "name");
            Assert.NotNull(key);
            Assert.Equal("name", key.Key);
            Assert.Equal("PD", key.Namespace);

            key = template.Keys.FirstOrDefault(k => k.Key == "code");
            Assert.NotNull(key);
            Assert.Equal("code", key.Key);
            Assert.Equal(string.Empty, key.Namespace);
        }
    }
}
