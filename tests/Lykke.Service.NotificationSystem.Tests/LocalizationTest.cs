using Lykke.Service.NotificationSystem.Domain.Models;
using Xunit;

namespace Lykke.Service.NotificationSystem.Tests
{
    public class LocalizationTest
    {
        [Fact]
        public void ParceLanguageToLocalization_WithoutRegion()
        {
            var lang = Localization.From("en");

            Assert.Equal("en", lang.LanguageCode);
            Assert.Equal(string.Empty, lang.LanguageRegion);
        }

        [Fact]
        public void ParceLanguageToLocalization_WithRegion()
        {
            var lang = Localization.From("en-us");

            Assert.Equal("en", lang.LanguageCode);
            Assert.Equal("us", lang.LanguageRegion);
        }

        [Fact]
        public void ParceLanguageToLocalization_NotCaseSencetive()
        {
            var lang = Localization.From("eN-US");

            Assert.Equal("en", lang.LanguageCode);
            Assert.Equal("us", lang.LanguageRegion);
        }

        [Fact]
        public void ParceLanguageToLocalization_Compare()
        {
            var lang1 = Localization.From("eN-US");

            var lang2 = Localization.From("en-us");

            var lang3 = Localization.From("en");

            Assert.True(lang1.Equals(lang2));

            Assert.False(lang1.Equals(lang3));
        }

        [Fact]
        public void ParceLanguageToLocalization_DefaultLocal()
        {
            var defaultLocal = Localization.Default;

            Assert.Equal("*", defaultLocal.LanguageCode);
            Assert.Equal(string.Empty, defaultLocal.LanguageRegion);
        }
    }
}
