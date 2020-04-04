namespace MAVN.Service.NotificationSystem.Client.Models.NotificationTemplate
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
    ///   en: English - default region
    /// </summary>
    public class LanguageDto
    {
        /// <summary>
        /// Localization code (tag)
        /// </summary>
        public string LocalizationCode { get; set; }

        /// <summary>
        /// Create language dto from provided localization code
        /// </summary>
        /// <param name="local">Localization code</param>
        /// <returns>Language</returns>
        public static LanguageDto From(string local)
        {
            return new LanguageDto {LocalizationCode = local};
        }
    }
}
