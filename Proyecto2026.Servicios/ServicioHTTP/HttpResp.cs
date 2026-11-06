using System;
using System.Net;
using System.Net.Http;

namespace Proyecto2026.Servicios.ServicioHTTP
{
    public class HttpResp<T> // maneja la respuesta del servidor y el error que pueda haber
    {
        public T? Dato { get; }
        public bool Error { get; }
        public HttpResponseMessage? Response { get; set; }

        // Campo de respaldo: guarda el mensaje explícito
        private string? _mensaje;

        public string Mensaje
        {
            get => ObtenerError();
            set => _mensaje = value;
        }

        public HttpResp(T? dato, bool error, HttpResponseMessage? response, string mensaje)
        {
            Dato = dato;
            Response = response;
            Error = error;
            Mensaje = mensaje;
        }

        public string ObtenerError()
        {
            // Leer el CAMPO, nunca la propiedad
            if (!string.IsNullOrEmpty(_mensaje))
                return _mensaje;

            if (Response == null)
                return "No se obtuvo respuesta del servidor";

            if (Response.IsSuccessStatusCode)
                return string.Empty;

            return MensajePorStatusCode(Response.StatusCode);
        }

        private static string MensajePorStatusCode(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.Continue => "La solicitud ha sido recibida y el proceso continúa.",
            HttpStatusCode.SwitchingProtocols => "El servidor está cambiando a un protocolo diferente.",
            HttpStatusCode.Processing => "El servidor está procesando la solicitud, pero aún no hay respuesta disponible.",
            HttpStatusCode.EarlyHints => "El servidor está enviando sugerencias tempranas al cliente.",
            HttpStatusCode.Ambiguous => "La solicitud es ambigua.",
            HttpStatusCode.Moved => "El recurso solicitado se ha movido a una nueva ubicación.",
            HttpStatusCode.Found => "El recurso solicitado se ha encontrado en una ubicación diferente.",
            HttpStatusCode.RedirectMethod => "El cliente debe reenviar la solicitud con el mismo método.",
            HttpStatusCode.NotModified => "El recurso no ha sido modificado.",
            HttpStatusCode.UseProxy => "El cliente debe utilizar un proxy para acceder al recurso.",
            HttpStatusCode.Unused => "El código de estado no se utiliza en la versión actual del protocolo.",
            HttpStatusCode.RedirectKeepVerb => "El cliente debe reenviar la solicitud con el mismo método.",
            HttpStatusCode.PermanentRedirect => "El recurso solicitado se ha movido permanentemente a una nueva ubicación.",
            HttpStatusCode.BadRequest => "La solicitud incorrecta - Intente nuevamente.",
            HttpStatusCode.Unauthorized => "No autorizado",
            HttpStatusCode.PaymentRequired => "Requerido",
            HttpStatusCode.Forbidden => "Prohibido - No tiene permiso para acceder a este recurso.",
            HttpStatusCode.NotFound => "Recurso no encontrado.",
            HttpStatusCode.MethodNotAllowed => "Método no permitido.",
            HttpStatusCode.NotAcceptable => "No aceptable - El servidor no puede generar una respuesta que sea aceptable para el cliente.",
            HttpStatusCode.ProxyAuthenticationRequired => "Se requiere autenticación de proxy.",
            HttpStatusCode.RequestTimeout => "Tiempo de espera de la solicitud",
            HttpStatusCode.Conflict => "La solicitud no se puede completar debido a un conflicto con el estado actual del recurso.",
            HttpStatusCode.Gone => "El recurso solicitado ya no está disponible en el servidor y no se conoce ninguna dirección de reenvío.",
            HttpStatusCode.LengthRequired => "Se requiere longitud - El servidor se niega a aceptar la solicitud sin un encabezado de longitud de contenido válido.",
            HttpStatusCode.PreconditionFailed => "La condición previa falló.",
            HttpStatusCode.RequestEntityTooLarge => "La entidad de la solicitud es demasiado grande.",
            HttpStatusCode.RequestUriTooLong => "La URI de la solicitud es demasiado larga.",
            HttpStatusCode.UnsupportedMediaType => "Tipo de medio no compatible.",
            HttpStatusCode.RequestedRangeNotSatisfiable => "Rango solicitado no satisfactorio.",
            HttpStatusCode.ExpectationFailed => "La expectativa de la solicitud falló.",
            HttpStatusCode.MisdirectedRequest => "Solicitud mal dirigida.",
            HttpStatusCode.UnprocessableEntity => "Entidad no procesable.",
            HttpStatusCode.Locked => "Recurso bloqueado.",
            HttpStatusCode.FailedDependency => "Dependencia fallida.",
            HttpStatusCode.UpgradeRequired => "Se requiere actualización.",
            HttpStatusCode.PreconditionRequired => "Se requiere una condición previa.",
            HttpStatusCode.TooManyRequests => "Demasiadas solicitudes.",
            HttpStatusCode.RequestHeaderFieldsTooLarge => "Los campos de encabezado de la solicitud son demasiado grandes.",
            HttpStatusCode.UnavailableForLegalReasons => "No disponible por razones legales.",
            HttpStatusCode.InternalServerError => "Error interno del servidor.",
            HttpStatusCode.NotImplemented => "No implementado.",
            HttpStatusCode.BadGateway => "Puerta de enlace incorrecta.",
            HttpStatusCode.ServiceUnavailable => "Servicio no disponible.",
            HttpStatusCode.GatewayTimeout => "Tiempo de espera de la puerta de enlace agotado.",
            HttpStatusCode.HttpVersionNotSupported => "Versión HTTP no compatible.",
            HttpStatusCode.VariantAlsoNegotiates => "La variante también negocia.",
            HttpStatusCode.InsufficientStorage => "Almacenamiento insuficiente.",
            HttpStatusCode.LoopDetected => "Bucle detectado.",
            HttpStatusCode.NotExtended => "No extendido.",
            HttpStatusCode.NetworkAuthenticationRequired => "Se requiere autenticación de red.",
            _ => $"Error HTTP {(int)statusCode}."
        };
    }
}