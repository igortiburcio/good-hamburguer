using System.Globalization;

namespace GoodHamburger.View.Services;

public static class CurrencyExtensions
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string ToBrl(this decimal value)
    {
        return value.ToString("C", PtBr);
    }
}
