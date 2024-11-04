using System.Data.SqlTypes;
using UnityEngine;

public class NumberScaleToText
{
    /// <summary>
    /// Convert a number from 0 to 10 to a text representation of a scale from 0 to 10.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string TenScaleToText(int number)
    {
        switch (number)
        {
            case 0:
                return "None";
            case 1:
                return "Very Low";
            case 2:
                return "Low";
            case 3:
                return "Moderate";
            case 4:
                return "Average";
            case 5:
                return "Normal";
            case 6:
                return "Above Average";
            case 7:
                return "High";
            case 8:
                return "Very High";
            case 9:
                return "Extreme";
            case 10:
                return "Maximum";
            default:
                return "Unknown";
        }
    }

    /// <summary>
    /// Convert a number from 0 to 100 to a text representation.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string HundredScaleToText(int number)
    {
        if(number == 0)
        {
            return "None";
        }
        // Go in steps of 10 instead
        return TenScaleToText(number / 10);
    }
}
