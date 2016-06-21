using BWF.Globalisation.Concrete;
using System.Collections.Generic;

namespace Brady.Trade.DataService.TestHost.Config
{
    public class Cultures
    {
        public readonly List<string> LanguageCultures;
        public readonly List<FormattingCulture> FormattingCultures;

        public Cultures()
        {
            LanguageCultures = new List<string> { "en-GB" };

            FormattingCultures = new List<FormattingCulture> {
                new FormattingCulture ("en-GB", new List<string> {
                    "dd/MM/yyyy HH:mm",
                    "dd MMMM yyyy HH:mm:ss",
                    "dd/MM/yyyy HH:mm:ss",
                    "dd-MM-yyyy HH:mm:ss",
                    "dd-MMM-yyyy HH:mm:ss",
                    "dd MMM yyyy HH:mm:ss"
                }, new List<string> {
                    "dd/MM/yyyy",
                    "dd MMMM yyyy",
                    "dd MMMM",
                    "MMMM yyyy",
                    "dd-MM-yyyy",
                    "dd-MMM-yyyy",
                    "dd MMM yyyy"
                })
            };
        }
    }
}
