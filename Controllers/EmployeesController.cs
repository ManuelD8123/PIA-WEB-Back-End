using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using PIA_BackEnd.Models;

namespace PIA_BackEnd.Controllers
{
    [Authorize]                     //Permite que cualquiera pueda solicitar esta informacion
    [RoutePrefix("api/Employees")]  //Ruta de donde se accedera a las peticiones del controlador

    //Activa CORS en todo el Controlador, en este caso se pueden realizar GET, PUT, POST, DELETE desde la direccion especificadas
    [EnableCors(origins: "https://localhost:44375", headers: "*", methods: "*")]

    public class EmployeesController : ApiController
    {
        /// <summary>
        /// Connexion al modelo de la base de Datos NorthWind
        /// </summary>
        private NorthWind db = new NorthWind();

        /// <summary>
        /// Obtiene toda la informacion de todos los empleados
        /// </summary>
        /// <returns>IQueryable con el contenido de todos los empleados</returns>
        // GET: api/Employees
        public IQueryable<Employees> GetEmployees()
        {
            //Realiza un consulta que retorna todos los empleados con toda su informacion
            return db.Employees.AsQueryable().Select(S => S);
        }

        /// <summary>
        /// Obtiene toda la informacion de un empleado ingresando el ID del mismo
        /// </summary>
        /// <param name="id">Entero numerico sin signo que indoca el Id del empleado a buscar</param>
        /// <returns>La informacion del primer y unico empleado el cual tiene asignado el ID ingresado</returns>
        // GET: api/Employees/"id"
        [ResponseType(typeof(Employees))]
        public IHttpActionResult GetEmployees(int id)
        {
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return NotFound();
            }

            return Ok(employees);
        }
        /// <summary>
        /// Edita la informacion de un Empleado cuya ID coincida con la proporcionada al metodo,
        /// todos los parametros de entrada con la excepcion del ID pueden estar vacios, en caso
        /// de estar vacio se usara el valor original del campo
        /// </summary>
        /// <param name="id">ID del empleaod, Solo lectura se utiliza para ubicar al usuario a editar</param>
        /// <param name="FirstName">Nombre del Empleado</param>
        /// <param name="LastName">Apellido del Empleado</param>
        /// <param name="TitleOfCourtesy">Titulo de Cortesia</param>
        /// <param name="Title">Titulo del Empleado</param>
        /// <param name="Address">Direccion del Empleado</param>
        /// <param name="City">Ciudad de residencia del epmpleado</param>
        /// <param name="Region">Region del Empleado</param>
        /// <param name="PostalCode">Codigo postal del Empleado</param>
        /// <param name="Country">Pais de residencia del Empleado</param>
        /// <param name="HomePhone">Telefono de Casa del Empleado</param>
        /// <param name="Extention">Extencion del numero de cada del Empleado</param>
        /// <param name="Note">Nota descriptiva sobre el Empleado</param>
        /// <param name="PhotoPath">Direccion URL que ubica la foto de dicho empleado</param>
        /// <returns></returns>
        //PUT: api/Employees/
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployees(int id,
            String FirstName = "", String LastName = "", String TitleOfCourtesy = "", String Title = "",
            String Address = "", String City = "", String Region = "", String PostalCode = "",
            String Country = "", String HomePhone = "", String Extention = "", String Note = "",
            String PhotoPath = "")
        {
            //Verifica que el estado del modelo siga siendo el mismo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Guarda temporalmente el empleado en memoria para verificar que se encontro un empleado
            Employees empleado = db.Employees.Find(id);

            if (empleado == null)
            {
                return NotFound();
            }

            //Pregunta si se asignaron o no y en caso de lo ultimo conserva la informacion original
            empleado.FirstName = String.IsNullOrEmpty(FirstName) ? empleado.FirstName : FirstName;
            empleado.LastName = String.IsNullOrEmpty(LastName) ? empleado.LastName : LastName;
            empleado.Title = String.IsNullOrEmpty(Title) ? empleado.Title : Title;
            empleado.TitleOfCourtesy = String.IsNullOrEmpty(TitleOfCourtesy) ? empleado.TitleOfCourtesy : TitleOfCourtesy;
            empleado.Address = String.IsNullOrEmpty(Address) ? empleado.Address : Address;
            empleado.City = String.IsNullOrEmpty(City) ? empleado.City : City;
            empleado.Region = String.IsNullOrEmpty(Region) ? empleado.Region : Region;
            empleado.PostalCode = String.IsNullOrEmpty(PostalCode) ? empleado.PostalCode : PostalCode;
            empleado.Country = String.IsNullOrEmpty(Country) ? empleado.Country : Country;
            empleado.HomePhone = String.IsNullOrEmpty(HomePhone) ? empleado.HomePhone : HomePhone;
            empleado.Extension = String.IsNullOrEmpty(Extention) ? empleado.Extension : Extention;
            empleado.Notes = String.IsNullOrEmpty(Note) ? empleado.Notes : Note;
            empleado.PhotoPath = String.IsNullOrEmpty(PhotoPath) ? empleado.PhotoPath : PhotoPath;
            //Ubica al Empleado en la base de datos y le asigna el estatus de Modificado pero no guarda los cambios
            db.Entry(empleado).State = EntityState.Modified;

            try
            {
                //Si todo esta bien se manda a cargar los cambios al Servidor
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
                {
                    //En caso que no se encuentre el usuario en la base de datos retorna un error
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Se ingresa un nuevo empleado a la base de datos
        /// 
        /// Solo los primero dos campos son obligatorios
        /// </summary>
        /// <param name="FirstName">Nombre del Empleado (Obligatorio)</param>
        /// <param name="LastName">Apellido del Empleado (Obligatorio)</param>
        /// <param name="TitleOfCourtesy">Titulo de Cortesia</param>
        /// <param name="Title">Titulo del Empleado</param>
        /// <param name="Address">Direccion del Empleado</param>
        /// <param name="City">Ciudad de residencia del epmpleado</param>
        /// <param name="Region">Region del Empleado</param>
        /// <param name="PostalCode">Codigo postal del Empleado</param>
        /// <param name="Country">Pais de residencia del Empleado</param>
        /// <param name="HomePhone">Telefono de Casa del Empleado</param>
        /// <param name="Extention">Extencion del numero de cada del Empleado</param>
        /// <param name="Note">Nota descriptiva sobre el Empleado</param>
        /// <param name="PhotoPath">Direccion URL que ubica la foto de dicho empleado</param>
        /// <returns></returns>
        // POST: api/Employees
        [ResponseType(typeof(Employees))]
        public IHttpActionResult PostEmployees(
            String FirstName, String LastName, String TitleOfCourtesy = "", String Title = "",
            String Address = "", String City = "", String Region = "", String PostalCode = "",
            String Country = "", String HomePhone = "", String Extention = "", String Note = "",
            String PhotoPath = "" 
            )
        {
            //Se verifica que el modelo aun coincide con la base de datos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Se crea un nuevo empleado con la informacion solicitada y la demas info en caso de no estar se le asigna NULL
            Employees nuevoEmpleado = new Employees
            {
                FirstName = FirstName,
                LastName = LastName,
                Title = String.IsNullOrEmpty(Title) ? null : Title,
                TitleOfCourtesy = String.IsNullOrEmpty(TitleOfCourtesy) ? null : TitleOfCourtesy,
                Address = String.IsNullOrEmpty(Address) ? null : Address,
                City = String.IsNullOrEmpty(City) ? null : City,
                Region = String.IsNullOrEmpty(Region) ? null : Region,
                PostalCode = String.IsNullOrEmpty(PostalCode) ? null : PostalCode,
                Country = String.IsNullOrEmpty(Country) ? null : Country,
                HomePhone = String.IsNullOrEmpty(HomePhone) ? null : HomePhone,
                Extension = String.IsNullOrEmpty(Extention) ? null : Extention,
                Notes = String.IsNullOrEmpty(Note) ? null : Note,
                PhotoPath = String.IsNullOrEmpty(PhotoPath) ? null : PhotoPath
            };

            //Se agrega a la lista de datos
            db.Employees.Add(nuevoEmpleado);
            //Se suben los cambios al servidor
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nuevoEmpleado.EmployeeID }, nuevoEmpleado);
        }
        /// <summary>
        /// Borra un empleado proporcionando su ID
        /// </summary>
        /// <param name="id">ID del empleado a eliminar de la base de datos</param>
        /// <returns></returns>
        // DELETE: api/Employees/id
        [ResponseType(typeof(Employees))]
        public IHttpActionResult DeleteEmployees(int id)
        {
            //Buscamos al Usuario
            Employees employees = db.Employees.Find(id);

            //Si no se encuentra se manda el aviso al servidor
            if (employees == null)
            {
                return NotFound();
            }

            //Se elimina de la lista de Datos
            db.Employees.Remove(employees);
            //Se realizan los cambios en la BAse de Datos
            db.SaveChanges();

            return Ok(employees);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeesExists(int id)
        {
            return db.Employees.Count(e => e.EmployeeID == id) > 0;
        }
    }
}