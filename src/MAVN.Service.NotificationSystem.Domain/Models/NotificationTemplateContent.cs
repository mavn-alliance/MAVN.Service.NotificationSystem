using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MAVN.Service.NotificationSystem.Domain.Models
{
    /// <summary>
    /// Template with content
    /// </summary>
    public class NotificationTemplateContent
    {
        public NotificationTemplateContent(string name, Localization localization, string content)
        {
            Name = name;
            Localization = localization;
            SetContent(content);
        }

        /// <summary>
        /// Template name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of available language
        /// </summary>
        public Localization Localization { get; }

        /// <summary>
        /// Template content
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// List of keys
        /// </summary>
        public IReadOnlyList<TemplateKey> Keys { get; private set; }

        public void SetContent(string content)
        {
            var keys = new Dictionary<string, TemplateKey>();

            var match = Regex.Match(content, @"\${(.+?)}", RegexOptions.IgnoreCase);

            while (match.Success)
            {
                var keytext = match.Result("$1");

                if (keytext.Contains("::"))
                {
                    var keymatch = Regex.Match(keytext, @"(.+?)::(.+?)$", RegexOptions.IgnoreCase);
                    if (keymatch.Success)
                    {
                        var key = new TemplateKey(keymatch.Result("$2"), keymatch.Result("$1"));
                        keys[$"{key.Namespace}::{key.Key}"] = key;
                    }
                }
                else
                {
                    var key = new TemplateKey(keytext);
                    keys[key.Key] = key;
                }

                match = match.NextMatch();
            }

            Content = content;
            Keys = keys.Values.ToList();
        }
    }
}
