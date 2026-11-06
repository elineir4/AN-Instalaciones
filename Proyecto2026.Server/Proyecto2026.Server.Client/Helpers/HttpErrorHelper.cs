using Proyecto2026.Servicios.ServicioHTTP;

namespace Proyecto2026.Server.Client.Helpers;

/// <summary>
/// Obtiene el mensaje legible de una respuesta fallida del servidor.
/// El backend devuelve los errores (400/404) como texto plano en el body,
/// a veces serializado como cadena JSON (entre comillas); si el body no sirve
/// (vacío o página HTML de excepción) se usa el respaldo de <see cref="HttpResp{T}.Mensaje"/>.
/// </summary>
public static class HttpErrorHelper
{
    public static async Task<string> ObtenerMensajeError<T>(HttpResp<T> resp)
    {
        if (resp.Response?.Content != null)
        {
            try
            {
                var body = await resp.Response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var trimmed = body.Trim();

                    // Cadena JSON entre comillas: "No existe el cliente con id 5"
                    if (trimmed.Length >= 2 && trimmed.StartsWith('"') && trimmed.EndsWith('"'))
                    {
                        trimmed = trimmed[1..^1].Trim();
                    }

                    // Página HTML (por ejemplo la de excepción en Development): no sirve como mensaje
                    if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith('<'))
                    {
                        return trimmed;
                    }
                }
            }
            catch
            {
                // Si la lectura del body falla se usa el respaldo de abajo
            }
        }

        // Respaldo: mensaje explícito o texto genérico del código de estado
        return resp.Mensaje;
    }
}
