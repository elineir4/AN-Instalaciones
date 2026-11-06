using System.Globalization;
using System.Text;

namespace Proyecto2026.Server.Client.Helpers;

public static class FormatoMonedaHelper
{
    private static readonly CultureInfo CulturaArgentina;

    static FormatoMonedaHelper()
    {
        // Se configura la cultura es-AR para asegurar el formato: $ 1.250.000,00
        CulturaArgentina = (CultureInfo)CultureInfo.GetCultureInfo("es-AR").Clone();
        CulturaArgentina.NumberFormat.CurrencySymbol = "$";
        CulturaArgentina.NumberFormat.CurrencyDecimalDigits = 2;
        CulturaArgentina.NumberFormat.CurrencyGroupSeparator = ".";
        CulturaArgentina.NumberFormat.CurrencyDecimalSeparator = ",";
        CulturaArgentina.NumberFormat.CurrencyPositivePattern = 0; // "$ n"
        CulturaArgentina.NumberFormat.CurrencyNegativePattern = 1; // "-$ n"
    }

    /// <summary>
    /// Formatea un valor monetario en formato argentino ($ 1.250.000,00).
    /// </summary>
    public static string FormatearMoneda(decimal monto)
    {
        return monto.ToString("C2", CulturaArgentina);
    }

    /// <summary>
    /// Remueve acentos y tildes de una cadena para comparaciones insensibles a diacríticos.
    /// </summary>
    public static string RemoverTildes(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
