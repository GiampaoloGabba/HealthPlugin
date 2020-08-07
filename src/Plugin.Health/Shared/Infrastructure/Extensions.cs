using System.Globalization;

namespace Plugin.Health
{
    internal static class StringExtensions
    {
        public static int ToInt(this string numero, int defVal = 0)
        {
            if (!int.TryParse(numero, out var num)) num = defVal;

            return num;
        }

        public static double ToDbl(this string numero, double defVal = 0)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return defVal;

            var style   = NumberStyles.Float;
            var culture = CultureInfo.CreateSpecificCulture("it-IT");

            if (!double.TryParse(numero.Replace(".", ","), style, culture, out var num)) num = defVal;

            return num;
        }
    }

}
