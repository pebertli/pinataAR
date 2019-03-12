
using System;

public class Util
{
    public static string FormatTimer(float time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string s = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

        return s;
    }
}
