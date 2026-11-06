using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Proyecto2026.Servicios.ServicioHTTP
{
    public class HttpService : IHttpService
    //aca voy a poner todo lo que vaya del cliente al servidor
    {
        private readonly HttpClient http;

        public HttpService(HttpClient http)
        {
            this.http = http;
        }

        //Metodo GET: sirve para obtener un registro del servidor
        public async Task<HttpResp<T>> GetAsync<T>(string url)
        {
            try
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<T>(default, true, response, null);
                }

                var dato = await response.Content.ReadFromJsonAsync<T>();

                return new HttpResp<T>(dato, false, response, null);
            }
            catch (Exception ex)
            {
                return new HttpResp<T>(default, true, null, ex.Message);

            }
        }

        //Metodo POST: sirve para enviar un registro nuevo al servidor
        public async Task<HttpResp<TResp>> PostAsync<T, TResp>(string url, T entidad)
       {
            try
            {
                var jsonEnviar = JsonSerializer.Serialize(entidad);//aca estoy serializando la entidad que me llega a json y luego la envio al servidor
                var cuerpo = new StringContent(jsonEnviar, Encoding.UTF8, "application/json");//aca estoy creando un objeto de tipo StringContent que contiene el json que voy a enviar al servidor

                var response = await http.PostAsync(url, cuerpo);//aca estoy enviando la entidad al servidor
                if (!response.IsSuccessStatusCode)//aca estoy verificando si la respuesta del servidor es exitosa o no
                {
                    return new HttpResp<TResp>(default, true, response, null);//aca estoy devolviendo un objeto de tipo HttpResp con el error que me devuelve el servidor
                }
                else //si la respuesta del servidor es exitosa, entonces deserializo la respuesta del servidor a un objeto de tipo TResp y lo devuelvo en un objeto de tipo HttpResp
                {
                    var dato = await response.Content.ReadFromJsonAsync<TResp>();//aca estoy deserializando la respuesta del servidor a un objeto de tipo TResp
                    return new HttpResp<TResp>(dato, false, response, null);//aca estoy devolviendo un objeto de tipo HttpResp con el dato que me devuelve el servidor
                }
              

              
            }
            catch (Exception ex)
            {

                return new HttpResp<TResp>(default, true, null, ex.Message);
            }
        }

        //Método PUT: sirve para actualizar un registro existente
        public async Task<HttpResp<TResp>> PutAsync<T, TResp>(string url, T entidad)
        {
            try
            {
                var jsonEnviar = JsonSerializer.Serialize(entidad);
                var cuerpo = new StringContent(jsonEnviar, Encoding.UTF8, "application/json");

                var response = await http.PutAsync(url, cuerpo);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResp>(default, true, response, null);
                }

                var dato = await response.Content.ReadFromJsonAsync<TResp>();
                return new HttpResp<TResp>(dato, false, response, null);
            }
            catch (Exception ex)
            {
                return new HttpResp<TResp>(default, true, null, ex.Message);
            }
        }

        // Método DELETE: sirve para eliminar un registro
        public async Task<HttpResp<TResp>> DeleteAsync<TResp>(string url)
        {
            try
            {
                var response = await http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResp>(default, true, response, null);
                }

                return new HttpResp<TResp>(default, false, response, null);
            }
            catch (Exception ex)
            {
                return new HttpResp<TResp>(default, true, null, ex.Message);
            }
        }
    }
}