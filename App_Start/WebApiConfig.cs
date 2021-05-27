using PIA_BackEnd.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PIA_BackEnd
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            config.EnableCors(); //Activa el uso de CORS para la API (Requiere Paquete NuGet)

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
