using System.Globalization;
using UnityEngine;

public static class NumberFormatter
{
    private static readonly string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

    public static string Format(float num, int maxDecimals = 2)
    {
        if (num == 0) return "0";
        if (num < 1000) 
        {
            return num.ToString("N0", CultureInfo.InvariantCulture); // 整數ไม่มี comma หรือเปลี่ยนเป็น "F0" ถ้าไม่ต้องการ comma
        }

        int suffixIndex = 0;
        while (num >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            num /= 1000;
            suffixIndex++;
        }

        // แสดงทศนิยมตามขนาด: <10 → 2 digits, >=10 → 1 digit
        int decimals = (num < 10) ? maxDecimals : 1;
        return num.ToString($"F{decimals}", CultureInfo.InvariantCulture) + suffixes[suffixIndex];
    }
}