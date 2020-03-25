using System;
using System.Text.RegularExpressions;
using Xunit;

namespace Lykke.Service.NotificationSystem.Tests
{
    public class MessageParametersTests
    {
        // This regex is same as the one inside MessageService and it is used inside ProcessMessageParameters
        // If the regular expression engine can find a match,
        // the first element of the GroupCollection object (the element at index 0)
        // returned by the Groups property contains a string that matches the entire regular expression pattern. 
        // Each subsequent element, from index one upward, represents a captured group.
        private readonly string _messageParametersRegexPattern = "^@@@(?<MessageParameters>.*?)@@@(?<MessageText>.*?)$";

        [Fact]
        public void
            When_Single_Line_Message_Template_Has_One_Message_Parameters_Section_Defined_At_The_Beginning_Then_Message_Parameters_And_Text_Are_Extracted()
        {
            var messageTemplate = "@@@{\"Param001\":\"param001data\"}@@@ some text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.True(match.Success);
            Assert.Equal(3, match.Groups.Count);
            Assert.Equal(messageTemplate, match.Groups[0].Value);
            Assert.Equal("{\"Param001\":\"param001data\"}", match.Groups[1].Value);
            Assert.Equal(" some text", match.Groups[2].Value);
        }

        [Fact]
        public void
            When_Multi_Line_Message_Template_Has_One_Message_Parameters_Section_Defined_At_The_Beginning_Then_Message_Parameters_And_Text_Are_Extracted()
        {
            var messageTemplate = "@@@{\"Param001\":\"param001data\"}@@@" +
                                  Environment.NewLine +
                                  " some text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.True(match.Success);
            Assert.Equal(3, match.Groups.Count);
            Assert.Equal(messageTemplate, match.Groups[0].Value);
            Assert.Equal("{\"Param001\":\"param001data\"}", match.Groups[1].Value);
            Assert.Equal(Environment.NewLine + " some text", match.Groups[2].Value);
        }

        [Fact]
        public void
            When_Single_Line_Message_Template_Has_One_Message_Parameters_Section_Not_Defined_At_The_Beginning_Then_Message_Parameters_Are_Not_Extracted()
        {
            var messageTemplate = "some text @@@{\"Param001\":\"param001data\"}@@@ some text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.False(match.Success);
        }

        [Fact]
        public void
            When_Multi_Line_Message_Template_Has_One_Message_Parameters_Section_Not_Defined_At_The_Beginning_Then_Message_Parameters_Are_Not_Extracted()
        {
            var messageTemplate = "some text @@@{\"Param001\":\"param001data\"}@@@" +
                                  Environment.NewLine +
                                  " some text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.False(match.Success);
        }

        [Fact]
        public void
            When_Single_Line_Message_Template_Has_Multiple_Message_Parameters_Sections_Defined_Then_Message_Parameters_Are_Extracted_From_The_Beginning_Of_Template_And_Everything_Else_Is_Extracted_As_Message_Text()
        {
            var messageTemplate =
                "@@@{\"Param001\":\"param001data\"}@@@ some text @@@{\"Param002\":\"param002data\"}@@@ some other text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.True(match.Success);
            Assert.Equal(3, match.Groups.Count);
            Assert.Equal(messageTemplate, match.Groups[0].Value);
            Assert.Equal("{\"Param001\":\"param001data\"}", match.Groups[1].Value);
            Assert.Equal(" some text @@@{\"Param002\":\"param002data\"}@@@ some other text", match.Groups[2].Value);
        }

        [Fact]
        public void
            When_Multi_Line_Message_Template_Has_Multiple_Message_Parameters_Sections_Defined_Then_Message_Parameters_Are_Extracted_From_The_Beginning_Of_Template_And_Everything_Else_Is_Extracted_As_Message_Text()
        {
            var messageTemplate = "@@@{\"Param001\":\"param001data\"}@@@" +
                                  Environment.NewLine +
                                  " some text" +
                                  Environment.NewLine +
                                  " some other text";
            var regex = new Regex(_messageParametersRegexPattern, RegexOptions.Singleline);

            var match = regex.Match(messageTemplate);

            Assert.True(match.Success);
            Assert.Equal(3, match.Groups.Count);
            Assert.Equal(messageTemplate, match.Groups[0].Value);
            Assert.Equal("{\"Param001\":\"param001data\"}", match.Groups[1].Value);
            Assert.Equal(Environment.NewLine + " some text" + Environment.NewLine + " some other text",
                match.Groups[2].Value);
        }
    }
}
