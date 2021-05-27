using PIA_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace PIA_BackEnd.Controllers
{
    [AllowAnonymous]    //Permite que se comuniquen sin importar quien realiza la comunicacion
                        //Al ser una pagina de LogIn es necesario que cualquiera pueda acceder
    [RoutePrefix("api/LogIn")] //Ruta de acceso de las peticiones
    public class LogInController : ApiController
    {
        /// <summary>
        /// Sirve para confirmar que se esta comunicando correctamente con la API
        /// </summary>
        /// <returns></returns>
        [HttpGet] //Se especifica que es un metodo Get para verificar que se puede comunicar con el resto de la API
        [Route("echoping")] //Ruta esoecifica de la solicitud
        public IHttpActionResult EchoPing()
        {
            return Ok(true); //Todo correcto :D
        }

        /// <summary>
        /// Pregunta si el usuario ya esta logeado actualmente
        /// </summary>
        /// <returns></returns>
        [HttpGet] //Se especifica que es un metodo Get para verificar que se puede comunicar con el resto de la API
        [Route("echouser")] //Ruta esoecifica de la solicitud
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        /// <summary>
        /// Nos permite enviar la informacion de inicio de session y nos regresa el Toquen JWT
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost] //Se especifica que es un metodo POST para verificar que se puede comunicar con el resto de la API
        [Route("authenticate")] //Nombre de la solicitud
        public IHttpActionResult Authenticate(LogRequest login)
        {
            //Verificamos que se enviara correctamente la informacion
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //Validamos credenciales, de momento no hay base de datos y cualquiera puede acceder usando esta contraseña
            //en un caso mas real se comunicaria a relaizar una consulta asi como un Hash a la contraseña para comparar
            //que si es la contraseña correcta
            bool isCredentialValid = (login.Password == "123456");
            if (isCredentialValid)
            {
                //Se Usa el nombre del Usuario para Generar el Token
                var token = TokenGenerator.GenerateTokenJwt(login.Username);

                //Se regresa el Estatus de OK junto con el token solicitado
                return Ok(token);
            }
            else
            {
                //Si no es valido regresamos un estatud de no autorizado
                return Unauthorized();
            }
        }
    }
}
