namespace MAVN.Service.NotificationSystem.Domain.Models
{
    /// <summary>
    /// Keys for template placeholders
    /// </summary>
    public class TemplateKey
    {
        public TemplateKey(string key, string ns)
        {
            Key = key;
            Namespace = ns;
        }

        public TemplateKey(string key)
        {
            Key = key;
            Namespace = string.Empty;
        }

        /// <summary>
        /// Key name
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// key namespace, can be null.
        /// </summary>
        public string Namespace { get; }

        public override string ToString()
        {
            return $"{Namespace}::{Key}";
        }
    }
}
