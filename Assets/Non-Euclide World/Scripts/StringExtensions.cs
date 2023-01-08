using System;

public static class StringExtensions
{
    public static string Cut(this string value, int bound1Index, int bound2Index, bool boundsInclude = false)
    {
        int firstIndex = bound1Index + Convert.ToInt32(!boundsInclude);
        return value.Substring(firstIndex, bound2Index - firstIndex + Convert.ToInt32(boundsInclude));
    }

    public static string CutFromFirst(this string value, int bound1Index, bool boundsInclude = false)
    {
        int firstIndex = bound1Index + Convert.ToInt32(!boundsInclude);
        return value.Substring(firstIndex, value.Length - firstIndex);
    }

    public static string CutFromSecond(this string value, int bound2Index, bool boundsInclude = false)
    {
        if (bound2Index < 0)
            return value;

        return value.Substring(0, bound2Index - Convert.ToInt32(!boundsInclude));
    }
}
