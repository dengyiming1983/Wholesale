using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class StringEx
{
    public static string PadLeftEx(this string str, int totalByteCount)
    {
        return PadLeftEx(str, totalByteCount, ' ');
    }
    public static string PadLeftEx(this string str, int totalByteCount, char c)
    {
        Encoding coding = Encoding.GetEncoding("gb2312");
        int dcount = 0;
        foreach (char ch in str.ToCharArray())
        {
            if (coding.GetByteCount(ch.ToString()) == 2)
                dcount++;
        }
        string w = str.PadRight(totalByteCount - dcount, c);
        return w;
    }

    public static string PadRightEx(this string str, int totalByteCount)
    {
        return PadRightEx(str, totalByteCount, ' ');
    }

    public static string PadRightEx(this string str, int totalByteCount, char c)
    {
        Encoding coding = Encoding.GetEncoding("gb2312");
        int dcount = 0;
        foreach (char ch in str.ToCharArray())
        {
            if (coding.GetByteCount(ch.ToString()) == 2)
                dcount++;
        }
        string w = str.PadRight(totalByteCount - dcount, c);
        return w;
    }


}

