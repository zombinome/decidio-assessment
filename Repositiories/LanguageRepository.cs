using System.Globalization;

namespace DecidioTestExcersice.Repositiories
{
    public class LanguageRepository
    {
        private static readonly IEnumerable<string> allLanguages = new[]
        {
            CultureInfo.GetCultureInfo("en-GB").DisplayName,
            CultureInfo.GetCultureInfo("fr-FR").DisplayName,
            CultureInfo.GetCultureInfo("de-AT").DisplayName,
        };

        public Task<IEnumerable<string>> GetAllLanguages()
        {
            return Task.FromResult(allLanguages);
        }
    }
}
