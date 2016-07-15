using System.Text.RegularExpressions;

namespace EPiLastic.Helpers
{
    public static class StringHelpers
    {
        const string HTML_TAG_PATTERN = "<.*?>";

        public static string StripHtml(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return string.Empty;

            return Regex.Replace
              (inputString, HTML_TAG_PATTERN, string.Empty);
        }
    }
}
