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
         * @return lista de proyectos
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
        public ActionResult Details(string id)//String id para conectar empleado con la tabla habilidades
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

        // GET: EmpleadoDesarrollador/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: EmpleadoDesarrollador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cedulaED,nombreED,apellido1ED,apellido2ED,fechaInicio,fechaNacimiento,edad,telefono,correo,disponibilidad,direccionExacta,distrito,canton,provincia,flg")] EmpleadoDesarrollador empleadoDesarrollador)
        {

                //Para válidar 
                if (ModelState.IsValid)
                {
                    //Valida si la cédula es nueva
                    if (!db.EmpleadoDesarrollador.Any(model => model.cedulaED == empleadoDesarrollador.cedulaED))
                    {
                    if (empleadoDesarrollador.fechaNacimiento != null)
                    {
                        DateTime fecha = empleadoDesarrollador.fechaNacimiento.Value;
                        int edad = System.DateTime.Now.Year - fecha.Year;
                        empleadoDesarrollador.edad = (byte)edad;
                    }
                    db.EmpleadoDesarrollador.Add(empleadoDesarrollador);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else {//Si la cédula ya existe, muestra mensaje de error
                        Response.Write("<script>alert('La cédula de este empleado ya existe. Intente con una nueva');</script>");//Si la cédula ya existe, muestra mensaje de error)
                }
                }
            return View(empleadoDesarrollador);
          }

        // GET: EmpleadoDesarrollador/Edit/5
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
        public ActionResult DeleteConfirmed(string id)
        {
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            db.EmpleadoDesarrollador.Remove(empleadoDesarrollador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
         //metodo que devuelve una lista con los empleados 
        public List<EmpleadoDesarrollador> getEmpleados()
        {
            var query = from EmpleadoDesarrollador in db.EmpleadoDesarrollador
                        select EmpleadoDesarrollador;
            return new List<EmpleadoDesarrollador>(query);
        }

        //Metodo que se encarga de cambiar la disponibilidad por medio de la cedula mandada por equipo
        public void modificarEstado(string cedula) {
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(cedula);
            empleadoDesarrollador.disponibilidad = false;
            db.SaveChanges();
        }
    }
}
