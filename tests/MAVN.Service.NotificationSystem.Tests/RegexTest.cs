using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace MAVN.Service.NotificationSystem.Tests
{
    public class RegexTest
    {

        private readonly ITestOutputHelper output;

        public RegexTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ExtractStringFromPlaceholder()
        {
            var input = "Hello ${111qweQQ qw} world ${222}.";

            Match match = Regex.Match(input, @"\${(.+?)}", RegexOptions.IgnoreCase);

            Assert.True(match.Success);
            output.WriteLine(match.Result("$1"));
            Assert.Equal("111qweQQ qw", match.Result("$1"));

            match = match.NextMatch();

            Assert.True(match.Success);
            output.WriteLine(match.Result("$1"));
            Assert.Equal("222", match.Result("$1"));

            match = match.NextMatch();

            Assert.False(match.Success);           
        }

        [Fact]
        public void ExtractKeyAndNamespace()
        {
            var input = "PD::FirstName";

            Match match = Regex.Match(input, @"(.+?)::(.+?)$");

            if (match.Success)
            {
                output.WriteLine(match.Result("$1"));
                output.WriteLine(match.Result("$2"));

                Assert.Equal("PD", match.Result("$1"));
                Assert.Equal("FirstName", match.Result("$2"));
            }

        }

        [Fact]
        public void Aggregate()
        {
            var list = new List<string>()
            {
                "1", "2", "3"
            };

            var s = list.Aggregate("", (c, i) => (string.IsNullOrEmpty(c) ? "" : (c + ";")) + i);

            output.WriteLine(s);
        }
    }
}
