using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using Newtonsoft.Json;



namespace Proyecto.Controllers
{
    
    public class RequerimientoController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        //instancias de controladores externos que se utilizan en en este
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.ModuloeController moduloController = new Proyecto.Controllers.ModuloeController();
        Proyecto.Controllers.EquipoController EqController = new Proyecto.Controllers.EquipoController();
        private static int modId; //variable golbal que se usa para mantener el modulo que es en el edit
        // GET: Requerimiento
        public async Task<ActionResult> Index()
        {
            //variables de sesión para obtener vista de acuerdo al rol
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;  
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            //variable para enviar a la vista
            ViewBag.user = usuario;

            //variables temporales que permiten el uso de los dropdown en las vistas, con valores ya predeterminados
            TempData["Estado"] = crearListaEstados();
            TempData["Complejidad"] = crearListaComplejidad();
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var requerimiento = db.Requerimiento.Include(r => r.EmpleadoDesarrollador).Include(r => r.Modulo);
                if (usuario == "Desarrollador") //si soy desarrollador veo los requerimientos asignados a mi
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.cedulaResponsable_FK == cedula
                                   select a;
                    return View(queryReq.ToList());
                }
                else if (usuario == "Lider") //si soy lider veo los requerimientos de mi proyecto
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK == proy
                                   select a;
                    return View(queryReq.ToList());
                }
                else if (usuario == "Cliente") { //si soy cliente veo los requerimientos de mi proyecto
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK == proy
                                   select a;
                    return View(queryReq.ToList());
                }
                return View(await requerimiento.ToListAsync());
                
            }
        }




        [HttpPost]
        public ActionResult Index(string nombreProyecto, string nombreModulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                //variables de sesion para saber sobre la información del usuario
                string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
                string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
                string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

                //variables para la vista
                ViewBag.proyectos = getProyectos(usuario, cedula);
                ViewBag.user = usuario;
                ViewBag.Modulo = nombreModulo;

                //busca el id del modulo que entra por parametro
                var queryMod = from a in db.Modulo
                               where a.NombreProy.Equals(nombreProyecto) && (a.Nombre.Equals(nombreModulo))
                               select a.Id;

                if (usuario == "Desarrollador") //Desarrollador ve requerimientos asignados a el
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK == nombreProyecto && a.idModulo_FK == queryMod.FirstOrDefault() && a.cedulaResponsable_FK == cedula
                                   select a;
                    return View(queryReq.ToList());
                }
                else //los otros usuarios ven requerimientos de sus proyectos
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK == nombreProyecto && a.idModulo_FK == queryMod.FirstOrDefault()
                                   select a;
                    return View(queryReq.ToList());
                }
            

            }
        }

        //conseguir el nombre de un empleado segun la cedula 
        public EmpleadoDesarrollador getNombre(string cedula)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var empleado = db.EmpleadoDesarrollador.Find(cedula);
                return empleado;
            }
        }


        public ActionResult consulReq(int modID,string nombreModulo, string nombreProy)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                //variables de sesion
                string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
                string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
                string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

                //variables para uso de vista 
                ViewBag.user = usuario;
                ViewBag.Proyecto = nombreProy;
                ViewBag.Modulo = nombreModulo;

                //busca el requerimiento por medio de sus llaves primarias
                var requerimiento = from Req in db.Requerimiento
                            where Req.idModulo_FK == modID && Req.nombreProyecto_FK == nombreProy
                            select Req;

                return View(requerimiento.ToList());

            }

        }

        


        // GET: Requerimiento/Details/5
         public async Task<ActionResult> Details(string nombreProy, int modID, string nombreReq)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                //obtiene el nombre del modulo por medio de sus llaves primarias
                ViewBag.nombreModulo = db.Modulo.Find(nombreProy, modID).Nombre;
                
                //busca el requerimiento por medio de sus llaves primarias
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(nombreProy,modID,nombreReq);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }

                //busca el responsable del requerimiento y en otro caso coloca que no se a asignado
                if (requerimiento.cedulaResponsable_FK != null)
                {
                    ViewBag.ApellidoEmpleado = db.EmpleadoDesarrollador.Find(requerimiento.cedulaResponsable_FK).apellido1ED;
                }
                else
                {
                    ViewBag.ApellidoEmpleado = "No asignado";
                }
               

                return View(requerimiento);
            }
        }

        // GET: Requerimiento/Create
        public ActionResult Create()
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                return View();
            }
        }

        // POST: Requerimiento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string nombreProyecto,string nombreModulo,  string miembro,string complejidad ,string estado,[Bind(Include = "nombreProyecto_FK,idModulo_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    //busca el id del nombre de modulo que entra por parametro
                    var queryMod = from a in db.Modulo
                                   where a.NombreProy.Equals(nombreProyecto) && (a.Nombre.Equals(nombreModulo))
                                   select a.Id;
                    //busca la cedula del nombre de responsable que entra por parametro
                    var queryResponsable = from b in db.EmpleadoDesarrollador
                                           where (b.nombreED.Equals(miembro))
                                           select b.cedulaED;
                    //uso del replace(evita tipos de inyecciones SQL)
                    string input = requerimiento.nombre;
                    string output = input.Replace("requerimiento", "");
                    requerimiento.nombre = output;

                    //asignacion
                    requerimiento.nombreProyecto_FK = nombreProyecto;
                    requerimiento.idModulo_FK = queryMod.FirstOrDefault();
                    requerimiento.cedulaResponsable_FK = queryResponsable.FirstOrDefault();

                    //agregado a la base de datos
                    db.Requerimiento.Add(requerimiento);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //actualizacion de datos de la vista para que no se pierdan 
                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", requerimiento.cedulaResponsable_FK);
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre", requerimiento.nombreProyecto_FK);
                return View(requerimiento);
            }
        }

     
        public ActionResult Edit(int ModID, string nombreProyecto, string nombreReq) // comunica con el modelo
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            modId = ModID; //se asigna esto a la variable global para en el metodo edit tipo post se conozca
            if (nombreReq == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //busco requerimiento
            Proyecto.Models.Requerimiento requerimiento = db.Requerimiento.Find(nombreProyecto, ModID, nombreReq);
            if (nombreProyecto == null)
            {
                return HttpNotFound();
            }
            //los modulos del proyecto respectivo para poder editar el modulo en el edit en dropdown
            List<SelectListItem> modulos = this.moduloController.getModulos(nombreProyecto).ToList();
            //los miembros del equipo del proyecto respectivo para poder editar el modulo en el edit en dropdown
            List<SelectListItem> equipo = this.EqController.getEmpleadosProyecto(nombreProyecto).ToList();
            //busco modulo actual
            Proyecto.Models.Modulo modulo = db.Modulo.Find(nombreProyecto, ModID);
            //busco empleado actual
            Proyecto.Models.EmpleadoDesarrollador desarrollador = db.EmpleadoDesarrollador.Find(requerimiento.cedulaResponsable_FK);

            //datos que se envian a la vista para desplegar en dropdown de seleccion
          
            //encuentro el usuario respectivo 
            var miembros = from a in db.Equipo
                                where a.nombreProy_FK == nombreProyecto
                                select a.EmpleadoDesarrollador.nombreED;

           
            TempData["Miembros"] = miembros.ToList();
         
            return View(requerimiento);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nombreProyecto_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento, string nombreProyecto, string miembros)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    //encuentro el id respectivo para modificarlo en requerimimiento
                    //encuentro el usuario respectivo 
                    var desarrollador = from a in db.Equipo
                                        where a.EmpleadoDesarrollador.nombreED == miembros
                                        && a.nombreProy_FK == requerimiento.nombreProyecto_FK
                                        select a.cedulaEM_FK;
                    //los modifico
                    requerimiento.idModulo_FK = modId;
                    requerimiento.cedulaResponsable_FK = desarrollador.FirstOrDefault();
                    //guardo el cambio en la base
                    db.Entry(requerimiento).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(requerimiento);
            }

        }

        public ActionResult Delete(string nombreProyecto, int modID, string nombreReq)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                //busca el modulo segun el id para obtener su nombre
                var mod = from a in db.Modulo
                          where a.Id == modID
                          select a.Nombre;

                //busca el responsable por su cedula para obtener su nombre
                var responsable = from a in db.EmpleadoDesarrollador
                                  from b in db.Requerimiento
                                  where a.cedulaED == b.cedulaResponsable_FK
                                  select a.nombreED;

                //con el nombre del responsable y modulos obtenidos arriba se utilizan estos tempdata para pasarselos a la vista
                TempData["ModuloAsociado"] = mod.FirstOrDefault();
                TempData["ResponsableAsociado"] = responsable.FirstOrDefault();
                Requerimiento requerimiento = db.Requerimiento.Find(nombreProyecto,modID,nombreReq);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }
                return View(requerimiento);
            }
        }

        //metodo tipo post de delete que se encarga de borrar luego de haber presionado submit
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string nombreProyecto, int modID, string nombreReq)
        {
            Requerimiento requerimiento = db.Requerimiento.Find(nombreProyecto,modID,nombreReq);
            db.Requerimiento.Remove(requerimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }




        
        public SelectList getProyectos(string rol, string cedula)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var listaproyectos = this.proyController.getProyectos(rol, cedula);
                return listaproyectos;
            }
        }

        //no se si volarmelo aqui dice que no se llama ni una vez
        public class Proyectito
        {
            public string nombreProyecto { get; set; }
        }
        

        //retorna un json con los modulos por proyecto que entra por parametro
        [HttpPost]
        public JsonResult getModulos(string nombreproyecto)
        {
                       
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                return Json(this.moduloController.getModulos(nombreproyecto));
            }
        }


        //Retorna los empleados segun el proyecto que entra por parametro
        [HttpPost]
        public JsonResult getEmpleadosProyecto(string nombreproyecto)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            { 
                return Json(this.EqController.getEmpleadosProyecto(nombreproyecto));
            }
        }

        //Crea la lista predefinida con las complejidades
        private List<string> crearListaComplejidad()
        {
            List<string> listaLocal = new List<string>();
            listaLocal.Add("Simple");
            listaLocal.Add("Mediano");
            listaLocal.Add("Complejo");
            listaLocal.Add("Muy Complejo");
            return listaLocal;
        }
        //Crea la lista predefinida con los estados
        private List<string> crearListaEstados()
        {
            List<string> listaLocal = new List<string>();
            listaLocal.Add("Sin iniciar");
            listaLocal.Add("En ejecucion");
            listaLocal.Add("Finalizado");
            listaLocal.Add("Suspendido");
            return listaLocal;
        }
    }
}
