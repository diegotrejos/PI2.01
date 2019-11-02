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
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.ModuloeController moduloController = new Proyecto.Controllers.ModuloeController();
        Proyecto.Controllers.EquipoController EqController = new Proyecto.Controllers.EquipoController();
        // GET: Requerimiento
        public async Task<ActionResult> Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            
            TempData["Complejidad"] = crearListaComplejidad();
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var requerimiento = db.Requerimiento.Include(r => r.EmpleadoDesarrollador).Include(r => r.Modulo);
                return View(await requerimiento.ToListAsync());
            }
        }




        [HttpPost]
        public ActionResult Index(string nombreProyecto, string nombreModulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
                ViewBag.user = usuario;
                string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
                string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
                ViewBag.proyectos = getProyectos(usuario, cedula);

              
                

                var queryMod = from a in db.Modulo
                               where a.NombreProy.Equals(nombreProyecto) && (a.Nombre.Equals(nombreModulo))
                               select a.Id;


                var queryReq = from a in db.Requerimiento
                               where a.nombreProyecto_FK == nombreProyecto && a.idModulo_FK == queryMod.FirstOrDefault()
                               select a;


                ViewBag.Modulo = nombreModulo;

                return View(queryReq.ToList());




            }
        }





        public ActionResult consulReq(int modID,string nombreModulo, string nombreProy)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
                ViewBag.user = usuario;
                string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
                string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;


                ViewBag.Proyecto = nombreProy;
                ViewBag.Modulo = nombreModulo;

                // var requerimiento = db.Requerimiento.Include(r => r.EmpleadoDesarrollador).Include(r => r.Modulo);
                //var requerimiento = reqController.getRequerimientos(modID, nombreProy, "BackLog");

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
                ViewBag.nombreModulo = db.Modulo.Find(nombreProy, modID).Nombre;
                

                Requerimiento requerimiento = await db.Requerimiento.FindAsync(nombreProy,modID,nombreReq);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }

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
                //  Proyecto.Controllers.EquipoController EqController = 
                //ViewBag.cedulaResponsable_FK = new Proyecto.Controllers.EquipoController().getEmpleadosProyecto();
               // ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre");
                return View();
            }
        }

        // POST: Requerimiento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string nombreProyecto,string nombreModulo,  string miembro,string complejidad ,[Bind(Include = "nombreProyecto_FK,idModulo_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    var queryMod = from a in db.Modulo
                                   where a.NombreProy.Equals(nombreProyecto) && (a.Nombre.Equals(nombreModulo))
                                   select a.Id;
                    var queryResponsable = from b in db.EmpleadoDesarrollador
                                           where (b.nombreED.Equals(miembro))
                                           select b.cedulaED;
                    string input = requerimiento.nombre;
                    string output = input.Replace("requerimiento", "");
                    requerimiento.nombre = output;
                    requerimiento.nombreProyecto_FK = nombreProyecto;
                    requerimiento.idModulo_FK = queryMod.FirstOrDefault();
                    requerimiento.cedulaResponsable_FK = queryResponsable.FirstOrDefault();
                    db.Requerimiento.Add(requerimiento);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", requerimiento.cedulaResponsable_FK);
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre", requerimiento.nombreProyecto_FK);
                return View(requerimiento);
            }
        }

         public ActionResult Edit(int ModId, string nombreProy,string nombreReq) // comunica con el modelo
        {
            //se obtiene el requerimiento que se va editar
            var query = from a in db.Modulo
                        from b in db.Requerimiento
                        where a.Id == ModId
                        select b;
            var item = query.FirstOrDefault(); //aqui se guarda el requerimiento que se busco

            //el nombre de proyecto llega nulo asi que se busca con este magiver
            var proy = from a in db.Modulo
                       where a.Id == ModId
                       select a.NombreProy;

            //los modulos del proyecto respectivo para poder editar el modulo en el edit
            var mods = from a in db.Modulo
                       where a.NombreProy == proy.FirstOrDefault()
                       select a.Nombre;

            //los posibles responsables a asignar
            var responsables = from a in db.Equipo
                              where a.nombreProy_FK == proy.FirstOrDefault()
                              select a.EmpleadoDesarrollador.nombreED;

            //variables utilizadas para pasar la informacion anterior a la vista
            TempData["Proyecto"] = proy.FirstOrDefault().ToList();

            TempData["Modulos"] = mods.ToList();

            TempData["Responsables"] = responsables.ToList(); 

            if (item != null)
                return View(item);
            else
                return View("NotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string responsable, string modulo,[Bind(Include = "nombreProyecto_FK,idModulo_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    //encuentro el id respectivo para modificarlo en requerimimiento
                    var mod = from a in db.Modulo
                              where a.NombreProy == requerimiento.nombreProyecto_FK
                              select a.Id;
                    //encuentro el usuario respectivo 
                    var desarrollador = from a in db.Equipo
                                        where a.EmpleadoDesarrollador.nombreED == responsable
                                        && a.nombreProy_FK == requerimiento.nombreProyecto_FK
                                        select a.cedulaEM_FK;
                    //los modifico
                    requerimiento.idModulo_FK = mod.FirstOrDefault();
                    requerimiento.cedulaResponsable_FK = desarrollador.FirstOrDefault();
                    //guardo el cambio en la base
                    db.Entry(requerimiento).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(requerimiento);
            }
        }
        
        // GET: Requerimiento/Delete/5
       public async Task<ActionResult> Delete(string nombreProyecto, int modID, string nombreReq)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
          
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(nombreProyecto,modID,nombreReq);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }
                return View(requerimiento);
            }
        }

        // POST: Requerimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(id);
                db.Requerimiento.Remove(requerimiento);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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

        
        public class Proyectito
        {
            public string nombreProyecto { get; set; }
        }
        
        [HttpPost]
        public JsonResult getModulos(string nombreproyecto)
        {
                       
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
             

                
               // Proyectito jsonData = JsonConvert.DeserializeObject<RequerimientoController.Proyectito>(nombreproyecto);

                // return Json(this.moduloController.getModulos(jsonData.nombreProyecto));
                return Json(this.moduloController.getModulos(nombreproyecto));
            }
        }


        [HttpPost]
        public JsonResult getEmpleadosProyecto(string nombreproyecto)
        {

            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {



                // Proyectito jsonData = JsonConvert.DeserializeObject<RequerimientoController.Proyectito>(nombreproyecto);

                // return Json(this.moduloController.getModulos(jsonData.nombreProyecto));
                return Json(this.EqController.getEmpleadosProyecto(nombreproyecto));
            }
        }

        private List<string> crearListaComplejidad()
        {
            List<string> listaLocal = new List<string>();
            listaLocal.Add("Simple");
            listaLocal.Add("Mediano");
            listaLocal.Add("Complejo");
            listaLocal.Add("Muy Complejo");
            return listaLocal;
        }


    }
}
