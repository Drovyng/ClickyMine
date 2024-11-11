using System.Globalization;

public static class Utils
{
    public static readonly CultureInfo Culture = new CultureInfo("en-US");

    public static float ParseFloat(string s)
    {
        return float.Parse(s, NumberStyles.Any, Culture.NumberFormat);
    }
}