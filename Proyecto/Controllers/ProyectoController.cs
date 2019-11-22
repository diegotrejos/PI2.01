using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Windows;

namespace Proyecto.Controllers
{
    public class ProyectoController : Controller
    {

        /*Variables que se utilizan en el inicio de sección para guardar datos necesarios*/
        public string usuario = "";     //Guarda el rol del usuario
        public string cedula = "";      //Guarda la cédula de la persona que entra
        public string proy = "";        //Guarda el proyecto en el que tiene participación la persona que entra


        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.EmpleadoDesarrolladorController empleadoController = new Proyecto.Controllers.EmpleadoDesarrolladorController();

        // GET: Proyecto

        /* Metodo que devuelve la lista de proyectos para la vista index
         * @return lista de proyectos
         */
        public ActionResult Index()
        {
            //Uso de variables temporales que guardan los datos necesarias para el inicio de sección
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            var proyecto = db.Proyecto.Include(p => p.Cliente);

            //Dependiendo del rol que se tenga se crean las consultas necesarias para devolver los datos del proyecto en el cual participan
            if (usuario == "Desarrollador" || usuario == "Lider") //Si es Lider o Desarrollador se devuelve el mismo proyecto dependiendo de su cédula
            {
                var obj = from a in db.Proyecto
                          from b in db.Equipo
                          where a.nombre == b.nombreProy_FK
                          where b.cedulaEM_FK == cedula
                          select a;

                return View(obj.ToList());

            }
            else if (usuario == "Cliente") //El cliente ve solo sus proyecto
            {
                var obj = from a in db.Proyecto
                          where a.cedulaCliente == cedula
                          select a;
                return View(obj.ToList());
            }
            else if (usuario == "Jefe")
            { //Como el Jefe puede ver todo, se le muestran todos los proyectos
                return View(proyecto.ToList());
            }
             return View();
        }

        // GET: Proyecto/Details/5
        /*Metodo que devuelve los detalles de un proyecto en específico
         * @param id : la llave del proyecto específico
         * @return proyecto
         */
        public ActionResult Details(string id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // GET: Proyecto/Create
        /*Metodo para la vista de la cedula de cliente
         * @return vista de cliente
         */
        public ActionResult Create()
        {
            
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre");
            return View();
        }

        // POST: Proyecto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*Método para crear un proyecto
  * @param datos necesarios para la creacion de un proyecto
  * @return vista del nuevo proyecto
  */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombre,duracionEstimada,costoTrabajo,costoEstimado,objetivo,fechaFinalizacion,fechaInicio,cedulaCliente")] Proyecto.Models.Proyecto proyecto, string lider)
        {


           
            if (ModelState.IsValid)
                {
                    if (!db.Proyecto.Any(model => model.nombre == proyecto.nombre))
                    {

                    /*  SqlConnection con = new SqlConnection("data source=172.16.202.23;user id=Gr02Proy3;password=Orion24!!!;MultipleActiveResultSets=True;App=EntityFramework");
                      SqlCommand cmd = new SqlCommand("SELECT * FROM Proyecto WHERE nombre=@nombre AND objetivo=@objetivo", con);
                      /* Convertimos en literal estos parámetros, por lo que no podrán hacer la inyección */
                    /* cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 15).Value = proyecto.nombre;
                     cmd.Parameters.Add("@objetivo", SqlDbType.VarChar, 256).Value = proyecto.objetivo;*/
                    var person = from b in db.EmpleadoDesarrollador
                                           where (b.nombreED.Equals(lider))
                                           select b.cedulaED;
                    db.Equipo.Add(new Equipo
                    {
                        cedulaEM_FK = person.FirstOrDefault(),
                        nombreProy_FK = proyecto.nombre,
                        rol = true
                    });

                    var item = from a in db.EmpleadoDesarrollador
                               where a.cedulaED == person.FirstOrDefault()
                               select a;
                    item.FirstOrDefault().disponibilidad = false;

                            db.Proyecto.Add(proyecto);
                        db.SaveChanges();
                        return RedirectToAction("");
                    }
                    else
                        Response.Write("<script>alert('El nombre del proyecto ya existe. Intente con uno nuevo');</script>");
                }


                ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
                return View(proyecto);
                //registro no existe
        }


        /*Método para la vista  editar un proyecto
         * @param id:llave del proyecto
         * @return vista del proyecto específico
         */
        // GET: Proyecto/Edit/5C:\Users\Katherine\Desktop\Proyecto\Proyecto\Controllers\ProyectoController.cs
        public ActionResult Edit(string id)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            var equipo = from a in db.Equipo
                         where a.nombreProy_FK == proyecto.nombre && a.rol == true
                         select a;
            var empleado = from a in db.EmpleadoDesarrollador
                           where equipo.FirstOrDefault().cedulaEM_FK == a.cedulaED
                           select a;
            List<SelectListItem> empl = getEmpledos();
            var newItem = new SelectListItem { Text = empleado.FirstOrDefault().nombreED, Value = empleado.FirstOrDefault().nombreED };
            empl.Add(newItem);
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
            ViewBag.cedulaLider = new SelectList(empl, "Value", "Text", empleado.FirstOrDefault().nombreED);


            return View(proyecto);
        }


        /*Metodo para editar un proyecto
         * @param datos necesarios para editar un proyecto
         * @return vista al inicio con los cambios del proyecto
         */
        // POST: Proyecto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nombre,duracionEstimada,costoTrabajo,costoEstimado,objetivo,fechaFinalizacion,fechaInicio,cedulaCliente")] Proyecto.Models.Proyecto proyecto, string lider)
        {
            
            if (ModelState.IsValid)
            {
              

                    db.Entry(proyecto).State = EntityState.Modified;
                    db.SaveChanges();

                var equipo = from a in db.Equipo
                             where a.nombreProy_FK == proyecto.nombre && a.rol == true
                             select a;
                var person = from b in db.EmpleadoDesarrollador
                             where (b.nombreED.Equals(lider))
                             select b.cedulaED;

                //equipo.FirstOrDefault().cedulaEM_FK = person.FirstOrDefault();
                
                return RedirectToAction("");


              
                   
            }
            var equip = from a in db.Equipo
                         where a.nombreProy_FK == proyecto.nombre && a.rol == true
                         select a;
            var empleado = from a in db.EmpleadoDesarrollador
                           where equip.FirstOrDefault().cedulaEM_FK == a.cedulaED
                           select a;

            List < SelectListItem > empl = getEmpledos();
            var newItem = new SelectListItem { Text = empleado.FirstOrDefault().nombreED, Value = empleado.FirstOrDefault().nombreED };
            empl.Add(newItem);
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
            ViewBag.cedulaLider = new SelectList(empl, "Value", "Text", empleado.FirstOrDefault().nombreED);

            return View(proyecto);
        }

        // GET: Proyecto/Delete/5
        /*Vista para eliminar un proyecto
         * @param id: llave del proyecto específico
         * @return vista del proyecto específico
         */
        public ActionResult Delete(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            var equipo = from a in db.Equipo
                         where a.nombreProy_FK == proyecto.nombre && a.rol == true
                         select a;
            var empleado = from a in db.EmpleadoDesarrollador
                           where equipo.FirstOrDefault().cedulaEM_FK == a.cedulaED
                           select a;
            ViewBag.lider = empleado.FirstOrDefault().nombreED;
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }



        // POST: Proyecto/Delete/5
        /*Método para borrar un proyecto
         *@param id: llave específica del proyecto
         * @return vista al inicio de proyecto borrado
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            db.Proyecto.Remove(proyecto);
            db.SaveChanges();
            return RedirectToAction("");
        }

        protected override void Dispose(bool disposing)
        {
           
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*Método para obtener una SelectLista de proyectos
         * @return lista de proyectos
         */
        public SelectList getProyectos(String rol, String cedula)
        {
            //Se hacen las consultas necesarias que devuelven el nombre del proyecto dependiendo del rol que se tiene
            if (rol == "Cliente") {
                var query = from proy in db.Proyecto
                            from cliente in db.Cliente
                            where proy.cedulaCliente == cliente.cedula
                            where cliente.cedula == cedula
                            select proy.nombre;
                return new SelectList(query);
            }
            else if (rol == "Empleado") {
                var query = from proy in db.Proyecto
                            from equip in db.Equipo
                            from emp in db.EmpleadoDesarrollador
                            where proy.nombre == equip.nombreProy_FK
                            where equip.cedulaEM_FK == emp.cedulaED
                            where emp.cedulaED == cedula
                            select proy.nombre;
                return new SelectList(query);
            }
            else {
                var query = from proy in db.Proyecto
                            select proy.nombre;
                return new SelectList(query);
            }
        }

        /*
         Método que retorna un SelectList con el nombre del proyecto
         */
        public SelectList getProyectos()
        {
                var query = from proy in db.Proyecto
                            select proy.nombre;
                return new SelectList(query);
        }

        /*Método para obtener una lista de proyectos
        * @return lista de proyectos
        */
        public List<Proyecto.Models.Proyecto> gettProyectos(String rol, String cedula)
        {
            //Se hacen las consultas necesarias que devuelven una lista con informacion del proyecto dependiendo del rol que se tiene
            if (rol == "Lider")
            {
                var query = from proy in db.Proyecto
                            from equip in db.Equipo
                            from emp in db.EmpleadoDesarrollador
                            where proy.nombre == equip.nombreProy_FK
                            where equip.cedulaEM_FK == emp.cedulaED
                            where emp.cedulaED == cedula
                            select proy;
                return new List<Proyecto.Models.Proyecto>(query);
            }
            else
            {
                var query = from proy in db.Proyecto
                            select proy;
                return new List<Proyecto.Models.Proyecto>(query);

            }
        }

        public SelectList getProyectosPorRol() {

            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            if (usuario == "Desarrollador" || usuario == "Lider") //Si es Lider o Desarrollador se devuelve el mismo proyecto dependiendo de su cédula
            {
                var obj = from a in db.Proyecto
                          from b in db.Equipo
                          where a.nombre == b.nombreProy_FK
                          where b.cedulaEM_FK == cedula
                          select a.nombre;

                return new SelectList(obj);

            }
            else if (usuario == "Cliente") //El cliente ve solo sus proyecto
            {
                var obj = from a in db.Proyecto
                          where a.cedulaCliente == cedula
                          select a.nombre;
                return new SelectList(obj);
            }
            else if (usuario == "Jefe")
            { //Como el Jefe puede ver todo, se le muestran todos los proyectos
                var obj = from a in db.Proyecto
                          select a.nombre;
                return new SelectList(obj);
            }
            SelectList lista = new SelectList("");
            return lista;

        }


        /*Método encargado de devolver los empleados desarrolladores disponibles 
       * @return list: lista de empleados desarrolladores disponibles
       */
        public List<SelectListItem> getEmpledos() {
            List<SelectListItem> list = empleadoController.getEmpledos();
            return list;

        }
    }
}
