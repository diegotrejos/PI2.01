using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;



namespace Proyecto.Controllers
{
    public class EmpleadoDesarrolladorController : Controller
    {
        //Variables para saber quien está logeado y a que proyecto pertenece
        public string usuario = "";//EL usuario que está en el sistema
        public string cedula = "";//La édula de quien esta en el sistema
        public string proy = "";//Proyecto al que pertenece quien esta en el sistema

        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: EmpleadoDesarrollador
        /* Se devuelve los empleados dependiendo del rol del sistema
         * @return lista empleados
         */
        public ActionResult Index()
        {
            //Estas variables guardan los datos del usuario en sesion
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            
            if ( usuario == "Lider")//Si el usuarion en sesion es lider
            {//Puede ver los empleados que pertenecen a su equipo
                         var info = from b in db.Equipo
                                    from c in db.Proyecto
                                    where b.cedulaEM_FK == cedula
                                    where c.nombre == b.nombreProy_FK
                                    select b;
                

                        var obj = from a in db.EmpleadoDesarrollador
                                  from b in db.Equipo
                                  where b.nombreProy_FK == info.FirstOrDefault().nombreProy_FK
                                  where b.cedulaEM_FK == a.cedulaED
                                  select a;
                
                return View(obj.Distinct().ToList());
            }
            else if (usuario == "Cliente")//Si el usuarion en sesion es cliente
            {//Solo puede ver los empleados que estan en su proyecto
                var obj = from a in db.EmpleadoDesarrollador
                          from b in db.Equipo
                          from c in db.Proyecto
                          where a.cedulaED == b.cedulaEM_FK
                          where c.nombre == b.nombreProy_FK
                          where c.cedulaCliente == cedula
                          select a;

                return View(obj.Distinct().ToList());
            }
            else if (usuario == "Desarrollador")//Si el usuarion en sesion es desarrollador
            {//solo puede ver su propia información y no la de otros desarrolladores
                var obj = from a in db.EmpleadoDesarrollador
                          where a.cedulaED == cedula
                          select a;

                return View(obj.ToList());
            }
            else if (usuario == "Jefe")//Si el usuarion en sesion es jefe
            {//Tiene acceso a todo el sistema
                return View(db.EmpleadoDesarrollador.ToList());
            }
            return View();//retorna la vista

        }

        // GET: EmpleadoDesarrollador/Details/5
        /*Metodo para consultas de empleado
         * @param id : llave de empleado
         * @return vista de empleado desarrollador
         */
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            //Saber que valor hay en disponibilidad para presentarlo al usuario de manera entendible
            if (empleadoDesarrollador.disponibilidad == true)//Si está disponible
            {
                ViewBag.disp = "true";//Le pasa a la vista un true
            }
            else {
                ViewBag.disp = "false";//Sino le pasa un false
            }
            //Saber que valor hay en la bandera de rol para presentarlo al usuario de manera entendible
            if (empleadoDesarrollador.flg == true)//Si la bandera está en true
            {
                ViewBag.rol = "true";//Se lo comunica a la vista
            }
            else
            {
                ViewBag.rol = "false";//Si esta en false lo comunica
            }


            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // GET: EmpleadoDesarrollador/Create
        /*Vista de create
        * @return vista
        */
        public ActionResult Create()
        {
            return View();
        }
        // POST: EmpleadoDesarrollador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*Crea empleado
          * @param atributos para crear empleado
          * @return vista de empleados
          */
        public ActionResult Create([Bind(Include = "cedulaED,nombreED,apellido1ED,apellido2ED,fechaInicio,fechaNacimiento,edad,telefono,correo,disponibilidad,direccionExacta,distrito,canton,provincia,flg")] EmpleadoDesarrollador empleadoDesarrollador)
        {

                //Para válidar 
                if (ModelState.IsValid)
                {
                    //Valida si la cédula eno existe
                    if (!db.EmpleadoDesarrollador.Any(model => model.cedulaED == empleadoDesarrollador.cedulaED))
                    {
                    if (empleadoDesarrollador.fechaNacimiento != null)//verifica si introdujeron fecha de nacimiento
                    {
                        DateTime fecha = empleadoDesarrollador.fechaNacimiento.Value;//Saca el valor de la fecha introducida
                        int edad = System.DateTime.Now.Year - fecha.Year;//Calcula la edad
                        empleadoDesarrollador.edad = (byte)edad;//La guarda en el atributo edad, convertida en bytes
                    }
                    db.EmpleadoDesarrollador.Add(empleadoDesarrollador);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else {//Si la cédula ya existe, muestra mensaje de error
                        Response.Write("<script>alert('La cédula de este empleado ya existe. Intente con una nueva');</script>");//Si la cédula ya existe, muestra mensaje de error)
                }
                }
            ViewBag.Emp = empleadoDesarrollador.fechaInicio;
            return View(empleadoDesarrollador);
          }

        // GET: EmpleadoDesarrollador/Edit/5
        /*Vista para editar uno
         * @param id: llave
         * @return vista de editar
         */
        public ActionResult Edit(string id)//String id para conectar empleado con la tabla habilidades
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // POST: EmpleadoDesarrollador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        /*Editar empleado
          * @param atributos para editar empleado
          * @return vista de empleados
          */
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cedulaED,nombreED,apellido1ED,apellido2ED,fechaInicio,fechaNacimiento,edad,telefono,correo,disponibilidad,direccionExacta,distrito,canton,provincia,flg")] EmpleadoDesarrollador empleadoDesarrollador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleadoDesarrollador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empleadoDesarrollador);
        }

        // GET: EmpleadoDesarrollador/Delete/5
        /*Vista para borrar empleado
          * @param llavedo
          * @return vista
          */
        public ActionResult Delete(string id)//String id para conectar empleado con la tabla habilidades
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // POST: EmpleadoDesarrollador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        /*Para confirmar si borrar empleado
          * @param llave
          * @return vista de la lista de empleados
          */
        public ActionResult DeleteConfirmed(string id)
        {
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            //Saber que valor hay en disponibilidad para presentarlo al usuario de manera entendible
            if (empleadoDesarrollador.disponibilidad == true)//Si está disponible
            {
                ViewBag.disp = "true";//Le pasa a la vista un true
            }
            else
            {
                ViewBag.disp = "false";//sino pasa a la vista un false
            }

            //Saber que valor hay en la bandera de rol para presentarlo al usuario de manera entendible
            if (empleadoDesarrollador.flg == true)//Si el rol es true
            {
                ViewBag.rol = "true";//Se lo comunica a la vista
            }
            else
            {
                ViewBag.rol = "false";//si esta en false lo comunica
            }
            db.EmpleadoDesarrollador.Remove(empleadoDesarrollador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*
          * 
          * @param 
          * @return 
          */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*
          *lista con los empleados 
          * @return devuelve la lista de empleado
          */
        public List<EmpleadoDesarrollador> getEmpleados()
        {
            var query = from EmpleadoDesarrollador in db.EmpleadoDesarrollador
                        select EmpleadoDesarrollador;
            return new List<EmpleadoDesarrollador>(query);
        }

        /*
          * Metodo que se encarga de cambiar la disponibilidad por medio de la cedula mandada por equipo
          * @param llave
          * @return 
          */
        public void modificarEstado(string cedula) {
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(cedula);
            empleadoDesarrollador.disponibilidad = false;
            db.SaveChanges();
        }

        /*Método encargado de devolver los empleados desarrolladores disponibles 
         * @return lista: lista de empleados desarrolladores disponibles
         */
        public List<SelectListItem> getEmpledos()
        {

            var item = from a in db.EmpleadoDesarrollador
                       where a.disponibilidad == true && a.flg == true
                       select new SelectListItem { Text = a.nombreED, Value = a.nombreED };
            List<SelectListItem> list = item.ToList();
            return list;

        }
    }
}
