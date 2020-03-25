namespace Lykke.Service.NotificationSystem.Domain.Models
{
    /// <summary>
    /// Language info, tag
    ///
    /// format 'langCode-regionCode'.
    /// langCode - language code in lower case. ISO 639-1.
    /// regionCode - region code in lower case, optional part. ISO 3166-1.
    /// https://sites.psu.edu/symbolcodes/web/langtag/
    /// 
    /// Example:
    ///   en-us: English - United States
    ///   en-in: English - India
    ///   en: English - defaul region
    /// </summary>
    public class Localization
    {
        public static readonly Localization Default = new Localization("*", string.Empty);

        public Localization(string languageCode, string languageRegion)
        {
            LanguageCode = languageCode;
            LanguageRegion = languageRegion;
        }

        /// <summary>
        /// lang code ISO 639-1
        /// </summary>
        public string LanguageCode { get; }

        /// <summary>
        /// lang region ISO 3166-1 (optional, by default = null)
        /// </summary>
        public string LanguageRegion { get; }

        public static Localization From(string language)
        {
            if (language.Contains('-'))
            {
                var prms = language.Split('-');
                return new Localization(prms[0].ToLower(), prms[1].ToLower());
            }
            return new Localization(language.ToLower(), string.Empty);
        }

        protected bool Equals(Localization other)
        {
            return string.Equals(LanguageCode, other.LanguageCode) && string.Equals(LanguageRegion, other.LanguageRegion);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Localization) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((LanguageCode != null ? LanguageCode.GetHashCode() : 0) * 397) ^ (LanguageRegion != null ? LanguageRegion.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(LanguageRegion))
                return $"{LanguageCode}-{LanguageRegion}";

            return LanguageCode;
        }
    }
}
