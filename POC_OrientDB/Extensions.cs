using System.Text.RegularExpressions;

namespace RegularExtensions
{
    public static class Extensions
    {

        public static bool Match(this string text, string regex)
        {
            return new Regex(regex).IsMatch(text);
        }

    }
}
