using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SEOAnalyser.Helpers
{
    public static class StringHelpers
    {
        public static bool IsValidUrl(this string value) {
            Uri uriResult;
            bool result = Uri.TryCreate(value, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public static string RemoveAllNonCharacters(this string value)
        {
            Regex rgx = new Regex("[^a-zA-Z -]");
            return rgx.Replace(value, string.Empty);
        }

        public static string RemoveAllMultipleWhiteSpaces(this string value)
        {
            Regex rgx = new Regex(@"\s{2,}");
            return rgx.Replace(value, " ");
        }

        public static string RemoveAllHtmlTags(this string value)
        {
            Regex rgx = new Regex(@"<[^>]+>|&nbsp;");
            return rgx.Replace(value, string.Empty);
        }

        public static string[] ToWordsArray(this string value)
        {
           return value.Split(null)?.Where(m => !m.Equals(string.Empty)).ToArray();
        }
    }
}