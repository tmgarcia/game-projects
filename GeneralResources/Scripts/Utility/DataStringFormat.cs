using System.Collections;

public class DataStringFormat 
{
    private string formatString;
    public string FormatString
    {
        get
        {
            return formatString;
        }
    }
    private DataStringFormat(string formatString)
    {
        this.formatString = formatString;
    }

    /// <summary>
    /// Format: Currency value
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>Format specifier as a string</returns>
    public static string CurrencyFormatString(int? numDecimalDigits)
    {
        string format = (numDecimalDigits.HasValue) ? "C" + numDecimalDigits : "C";
        return format;
    }
    /// <summary>
    /// Format: Currency value
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Currency(int? numDecimalDigits)
    {
        return new DataStringFormat(CurrencyFormatString(numDecimalDigits));
    }

    /// <summary>
    /// Format: Integer Digits with optional negative sign
    /// <para>Supported by: Integral types only</para>
    /// </summary>
    /// <param name="minNumDigits">Minimum number of digits</param>
    /// <returns>Format specifier as a string</returns>
    public static string DecimalFormatString(int? minNumDigits)
    {
        string format = (minNumDigits.HasValue) ? "D" + minNumDigits : "D";
        return format;
    }
    /// <summary>
    /// Format: Integer Digits with optional negative sign
    /// <para>Supported by: Integral types only</para>
    /// </summary>
    /// <param name="minNumDigits">Minimum number of digits</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Decimal(int? minNumDigits)
    {
        return new DataStringFormat(DecimalFormatString(minNumDigits));
    }

    /// <summary>
    /// Format: Exponential notation
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>Format specifier as a string</returns>
    public static string ExponentialFormatString(int? numDecimalDigits)
    {
        string format = (numDecimalDigits.HasValue) ? "E" + numDecimalDigits : "E";
        return format;
    }
    /// <summary>
    /// Format: Exponential notation
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Exponential(int? numDecimalDigits)
    {
        return new DataStringFormat(ExponentialFormatString(numDecimalDigits));
    }

    /// <summary>
    /// Format: Integral and decimal digits with optional negative sign
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>Format specifier as a string</returns>
    public static string FixedPointFormatString(int? numDecimalDigits)
    {
        string format = (numDecimalDigits.HasValue) ? "F" + numDecimalDigits : "F";
        return format;
    }
    /// <summary>
    /// Format: Integral and decimal digits with optional negative sign
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalDigits">Number of decimal digits</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat FixedPoint(int? numDecimalDigits)
    {
        return new DataStringFormat(FixedPointFormatString(numDecimalDigits));
    }

    /// <summary>
    /// Format: Most compact of either fixed-point or scientific notation
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numSignificantDigits">Number of signficiant digits</param>
    /// <returns>Format specifier as a string</returns>
    public static string GeneralFormatString(int? numSignificantDigits)
    {
        string format = (numSignificantDigits.HasValue) ? "G" + numSignificantDigits : "G";
        return format;
    }
    /// <summary>
    /// Format: Most compact of either fixed-point or scientific notation
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numSignificantDigits">Number of signficiant digits</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat General(int? numSignificantDigits)
    {
        return new DataStringFormat(GeneralFormatString(numSignificantDigits));
    }

    /// <summary>
    /// Format: Integral and decimal digits separated by commas, decimal separator, optional negative sign
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalPlaces">Desired number of decimal places</param>
    /// <returns>Format specifier as a string</returns>
    public static string NumberFormatString(int? numDecimalPlaces)
    {
        string format = (numDecimalPlaces.HasValue) ? "N" + numDecimalPlaces : "N";
        return format;
    }
    /// <summary>
    /// Format: Integral and decimal digits separated by commas, decimal separator, optional negative sign
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalPlaces">Desired number of decimal places</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Number(int? numDecimalPlaces)
    {
        return new DataStringFormat(NumberFormatString(numDecimalPlaces));
    }

    /// <summary>
    /// Format: Number multiplied by 100 and displayed with a percent symbol
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalPlaces">Desired number of decimal places</param>
    /// <returns>Format specifier as a string</returns>
    public static string PercentFormatString(int? numDecimalPlaces)
    {
        string format = (numDecimalPlaces.HasValue) ? "P" + numDecimalPlaces : "P";
        return format;
    }
    /// <summary>
    /// Format: Number multiplied by 100 and displayed with a percent symbol
    /// <para>Supported by: All numeric types</para>
    /// </summary>
    /// <param name="numDecimalPlaces">Desired number of decimal places</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Percent(int? numDecimalPlaces)
    {
        return new DataStringFormat(PercentFormatString(numDecimalPlaces));
    }

    /// <summary>
    /// Format: A hexidecimal string
    /// <para>Supported by: Integral types only</para>
    /// </summary>
    /// <param name="numDigits">Number of digits in the result string</param>
    /// <returns>Format specifier as a string</returns>
    public static string HexadecimalFormatString(int? numDigits)
    {
        string format = (numDigits.HasValue) ? "X" + numDigits : "X";
        return format;
    }
    /// <summary>
    /// Format: A hexidecimal string
    /// <para>Supported by: Integral types only</para>
    /// </summary>
    /// <param name="numDigits">Number of digits in the result string</param>
    /// <returns>DataStringFormat string wrapper</returns>
    public static DataStringFormat Hexadecimal(int? numDigits)
    {
        return new DataStringFormat(HexadecimalFormatString(numDigits));
    }
}
