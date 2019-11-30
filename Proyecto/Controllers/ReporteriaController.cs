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
using Newtonsoft.Json;

namespace Proyecto.Controllers
{
    public class ReporteriaController : Controller
    {
        //variables de sesión para obtener vista de acuerdo al rol
        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.EmpleadoDesarrolladorController emplController = new Proyecto.Controllers.EmpleadoDesarrolladorController();
        Proyecto.Controllers.HabilidadesController habController = new Proyecto.Controllers.HabilidadesController();
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;//Saber rol del que está en el sistema
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;//Proyecto del que está en el sistema
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;//Cédula del que está en el sistema
            ViewBag.user = usuario;
            return View();
        }


        public ActionResult TiemposRequerimiento()
        {
            ViewBag.nombreP = "Seleccione un proyecto";
            /* var item = (from a in db.Requerimiento
                         from b in db.EmpleadoDesarrollador
                         where a.cedulaResponsable_FK == b.cedulaED
                         select new { nombre = b.nombreED, req = a.nombre, suma = a.duracionEstimada - a.duracionReal });*/
            List<string> lista = new List<string>();
            /* foreach (var dato in item)
             {
                 lista.Add(dato.nombre + " " + dato.req + " " + dato.suma);
             }*/

            return View(lista);
        }

        [HttpPost]
        public ActionResult tablaRequerimientos(string nombre, string nombreProyecto)
        {
            System.Diagnostics.Debug.WriteLine("Desarrollador " + nombre);
            System.Diagnostics.Debug.WriteLine("el nombre " + nombreProyecto);
            ViewBag.todos = "";
            ViewBag.nombreP = "Seleccione un proyecto";
            var cedula = from a in db.EmpleadoDesarrollador
                         where a.nombreED == nombre
                         select a;
            var item = (from a in db.Requerimiento
                        where a.cedulaResponsable_FK == cedula.FirstOrDefault().cedulaED && a.nombreProyecto_FK == nombreProyecto
                        select new { nombre = a.nombreProyecto_FK, complejidad = a.complejidad,req = a.nombre, duracionEst = a.duracionEstimada, duracionReal = a.duracionReal, diferencia = a.duracionEstimada - a.duracionReal });

            if (nombreProyecto == "Todos los proyectos") {
                ViewBag.todos = "si";
                ViewBag.nombreP = "Todos los proyectos";
               item = (from a in db.Requerimiento
                       from b in db.Proyecto
                    where a.cedulaResponsable_FK == cedula.FirstOrDefault().cedulaED && b.nombre == a.nombreProyecto_FK && b.fechaFinalizacion != null
                    select new { nombre = a.nombreProyecto_FK, complejidad = a.complejidad, req = a.nombre, duracionEst = a.duracionEstimada, duracionReal = a.duracionReal, diferencia = a.duracionEstimada - a.duracionReal });
    
            }

            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                if (nombreProyecto != "Todos los proyectos")
                    ViewBag.nombreP = dato.nombre;
                lista.Add(dato.nombre + "," + dato.req + "," +dato.complejidad + "," + dato.duracionEst + "," + dato.duracionReal + "," +dato.diferencia );
            }

            return View(lista);
        }

        public ActionResult HorasProyecto()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;

            var item =
                           from b in db.Requerimiento
                           from a in db.Proyecto
                           from e in db.Equipo
                           from l in db.EmpleadoDesarrollador
                           where b.nombreProyecto_FK == a.nombre && e.nombreProy_FK == a.nombre && e.rol == true && e.cedulaEM_FK == l.cedulaED 
                           group new { a,b, e, l } by a.nombre into c
                           select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
            if (usuario == "Lider") {
                item =
                         from b in db.Requerimiento
                         from a in db.Proyecto
                         from e in db.Equipo
                         from l in db.EmpleadoDesarrollador
                         where b.nombreProyecto_FK == a.nombre  && e.cedulaEM_FK == cedula && e.rol == true && e.nombreProy_FK == a.nombre  &&  a.fechaFinalizacion != null
                         group new { a, b, e, l } by a.nombre into c
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };

            }
            if (item != null)
                item.Distinct();
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + ","+dato.lider+ "," + dato.sumaEst + "," + dato.sumaReal + "," + dato.dif);
            }

            return View(lista);
        }

        [HttpPost]
        public ActionResult HorasProyecto(string nombreProyecto)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            var item =
                           from b in db.Requerimiento
                           from a in db.Proyecto
                           from e in db.Equipo
                           from l in db.EmpleadoDesarrollador
                           where a.nombre == nombreProyecto && b.nombreProyecto_FK == nombreProyecto && e.nombreProy_FK == nombreProyecto && e.rol == true && e.cedulaEM_FK == l.cedulaED
                           group new { a, b, e, l } by a.nombre into c
                           select new { nombre = c.FirstOrDefault().a.nombre, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
            if (item != null)
                item.Distinct();
            if (nombreProyecto =="") {
                 item =
                         from b in db.Requerimiento
                         from a in db.Proyecto
                         from e in db.Equipo
                         from l in db.EmpleadoDesarrollador
                         where b.nombreProyecto_FK == a.nombre && e.nombreProy_FK == a.nombre && e.rol == true && e.cedulaEM_FK == l.cedulaED
                         group new { a, b, e, l } by a.nombre into c
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
                if (item != null)
                    item.Distinct();
            }
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + "," + dato.lider + "," + dato.sumaEst + "," + dato.sumaReal + "," + dato.dif);
            }

            return View(lista);
        }

     

        public SelectList getDesarrolladores()
        {
   
            return emplController.getDesarrolladores();
        }

        [HttpPost]
        public JsonResult getProyectoDesarrollador(string nombre)
        {
            //System.Diagnostics.Debug.WriteLine("el nombre" + nombre);
            var empleado = from a in db.EmpleadoDesarrollador
                           where a.nombreED == nombre
                           select a.cedulaED;
       
            var item = from a in db.Proyecto
                       from b in db.Equipo
                       where a.nombre == b.nombreProy_FK && a.fechaFinalizacion != null
                       where b.cedulaEM_FK == empleado.FirstOrDefault()
                       select a.nombre;
            if (item != null)
                item.Distinct();
            return  Json(new SelectList(item));
        }

        public SelectList getProyectos()
        {
            return proyController.getProyectosPorRol();
        }


        //ya terminado
        public ActionResult Estadodesarrollorequerimientos()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string nombreProyecto = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;

            if (usuario == "Desarrollador")
            { var query = from a in db.Requerimiento
                              //solo requerimientos en ejecucion o sin iniciar
                          where (a.nombreProyecto_FK == nombreProyecto)
                          where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                          where (a.cedulaResponsable_FK == cedula)
                          select a;
                return View(query.ToList());
            }



            else if (usuario == "Lider")
            {
                var query = from a in db.Requerimiento
                                //solo requerimientos en ejecucion o sin iniciar
                            where (a.nombreProyecto_FK == nombreProyecto)
                            where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                            select a;
                return View(query.ToList());
            }

            else
            {
                var query = from a in db.Requerimiento
                                //solo requerimientos en ejecucion o sin iniciar
                            where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                            select a;
                return View(query.ToList());
            }
        }


    //objeto para imprimir resultados bonitos
        public class Desocupacion
            {
            public
            string nombreED;
            public
           string periodosyDias;
            public
           int total;

        }


        //en progreso
        public ActionResult PeriodosDisponibles()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;
            List<Desocupacion> Empleados = new List<Desocupacion>();

            Desocupacion p = new Desocupacion();
            p.nombreED = "Diego";
            p.periodosyDias = "1/02/2006-2/03/2006 = 32";
            p.total = 32;
            //los agrego a la lista 
            Empleados.Add(p);
            return View(Empleados);
           
        }

        [HttpPost]
        public ActionResult PeriodosDisponibles(string Finicio, string Ffinal)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            List<Desocupacion> Empleados = new List<Desocupacion>();
            Finicio = "10/04/2005";
            Ffinal = "2/12/2006";



            //Genero lista de todos los empleados


            //Hago consulta aqui de sus datos
            for (int i = 0; i < 2; i++)
            {
                Desocupacion p = new Desocupacion();
                p.nombreED = "Diego";
                p.periodosyDias = "1/02/2006-2/03/2006 = 32";
                p.total = 32;
                //los agrego a la lista 
                Empleados.Add(p);
            }


            return View(Empleados);
        }


        //Comparación de tiempos con mismo nivel de complejidad, no recibe parámetros para mostrar información de todos los niveles
        /* consulta para obtener la cantidad total de requerimientos, el mínimo y máximo de la diferencia entre la duración estimada y la real y el tiempo promedio en horas de la real
         * @return lista 
         */
        public ActionResult ComparacionComplejidad()
        {
           var item = from req in db.Requerimiento
                      from proy in db.Proyecto
                      where proy.nombre == req.nombreProyecto_FK
                      where proy.fechaFinalizacion != null
                      group req by 1 into g
                      select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = g.Average(c => c.duracionReal) };

            //Lista que guarda las tuplas obtenidas
            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.total + " " + dato.minimo + " " + dato.maximo + " " + dato.promedio);
            }
            return View(datos);
        }

        //Comparación de tiempos con mismo nivel de complejidad que recibe como parámetro el nivel
        /* consulta para obtener la cantidad total de requerimientos, el mínimo y máximo de la diferencia entre la duración estimada y la real y el tiempo promedio en horas de la real
         * @param complejidad: el nivel de complejidad del requerimiento
         * @return lista 
         */
        [HttpPost]
        public ActionResult ComparacionComplejidad(string complejidad)
        {
            var item = from req in db.Requerimiento
                       from proy in db.Proyecto
                       where proy.nombre == req.nombreProyecto_FK
                       where req.complejidad == complejidad
                       where proy.fechaFinalizacion != null
                       group req by req.complejidad into g
                       select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = g.Average(c => c.duracionReal) };

            //Devuelve la información de todos los niveles
            if (complejidad == "" || complejidad == "Todos los niveles")
            {
                item = from req in db.Requerimiento
                       from proy in db.Proyecto
                       where proy.nombre == req.nombreProyecto_FK
                       where proy.fechaFinalizacion != null
                       group req by 1 into g
                       select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = g.Average(c => c.duracionReal) };
            }

            //Lista que guarda las tuplas obtenidas
            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.total + " " + dato.minimo + " " + dato.maximo + " " + dato.promedio);
            }
            return View(datos);
        }

        //Historial de desarrollador en proyectos, recibe como parámetro el nombre del empleado
        /* consulta para obtener el nombre de los proyectos en los que ha trabajado, su rol y total de horas dedicadas
         * @param nombre: el nombre del desarrollador
         * @return lista 
         */
        public ActionResult HistorialDesarrollador(string nombre)
        {
            var item = from proy in db.Proyecto
                       from req in db.Requerimiento
                       from eq in db.Equipo
                       from emp in db.EmpleadoDesarrollador
                       where emp.nombreED == nombre
                       where emp.cedulaED == req.cedulaResponsable_FK
                       where req.nombreProyecto_FK == proy.nombre
                       where proy.nombre == eq.nombreProy_FK
                       where eq.cedulaEM_FK == emp.cedulaED
                       where proy.fechaFinalizacion != null
                       group new { proy, req, eq } by new { proy.nombre, eq.rol } into g
                       select new { nom = g.Key.nombre, r = g.Key.rol, duracion = g.Sum(x => x.req.duracionReal) };
            
            //Lista que guarda las tuplas obtenidas
            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.nom + " " + dato.r + " " + dato.duracion);
            }
            return View(datos);
        }

        /*Crea la lista con los niveles de complejidad
         * @return lista
         */
        public List<SelectListItem> getListaComplejidad()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "Simple" });
            items.Add(new SelectListItem() { Text = "Mediano" });
            items.Add(new SelectListItem() { Text = "Complejo" });
            items.Add(new SelectListItem() { Text = "Muy Complejo" });
            return items;
        }

        /*Retorna los nombres de los desarrolladores para mostrar su historial
         * @return lista
         */
        public List<SelectListItem> getDesarrolladoresHistorial()
        {
            return emplController.getDesarrolladoresHistorial();
        }


        // DESARROLLADOR POR CONOCIMIENTO sin parámetro para todos los conocimientos
        /* consulta para obtener el numero de empleador por conocimiento y el promedio de tiempo trabajar en la empresa
         * @return lista 
         */
        public ActionResult DesarrolladoresPorConocimiento()
        {
            var now = DateTime.Now;//Para sacar el average de estadía de empresa 
            //consulta para conocimiento específico
           
            var    item = (from habi in db.Habilidades
                        from emp in db.EmpleadoDesarrollador
                        where habi.cedulaEmpleadoPK_FK == emp.cedulaED
                        group new { emp, habi } by new { conocimientos = habi.conocimientos } into g
                        orderby g.Key.conocimientos ascending
                        select new
                        {
                            nombre = g.Key.conocimientos,
                            cantDesa = g.Count(),
                            promedio = (int)g.Average(x => DbFunctions.DiffYears(x.emp.fechaInicio, now))
                        });

            
            //Lista con datos recibidos de la consulta
            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.nombre + " " + dato.cantDesa + " " + dato.promedio);
            }

            return View(datos);//retorna la vista
        }

        // DESARROLLADOR POR CONOCIMIENTO
        /* consulta para obtener el numero de empleador por conocimiento y el promedio de tiempo trabajar en la empresa
         * @return lista 
         */
        [HttpPost]
        public ActionResult DesarrolladoresPorConocimiento(string Habilidad)
        {
            var now = DateTime.Now;//Para sacar el average de estadía de empresa 
            //consulta para conocimiento específico
            var item = (from habi in db.Habilidades
                        from emp in db.EmpleadoDesarrollador
                        where habi.cedulaEmpleadoPK_FK == emp.cedulaED
                        where habi.conocimientos == Habilidad
                        group new { emp, habi } by new { conocimientos = habi.conocimientos } into g
                        orderby g.Key.conocimientos ascending
                        select new
                        {
                            nombre = g.Key.conocimientos,
                            cantDesa = g.Count(),
                            promedio = (int)g.Average(x => DbFunctions.DiffYears(x.emp.fechaInicio, now))
                        });

            //Consulta para todos los conocimientos
            if (Habilidad == "" || Habilidad == "Todos los conocimientos")
            {
                item = (from habi in db.Habilidades
                        from emp in db.EmpleadoDesarrollador
                        where habi.cedulaEmpleadoPK_FK == emp.cedulaED
                        group new { emp, habi } by new { conocimientos = habi.conocimientos } into g
                        orderby g.Key.conocimientos ascending
                        select new
                        {
                            nombre = g.Key.conocimientos,
                            cantDesa = g.Count(),
                            promedio = (int)g.Average(x => DbFunctions.DiffYears(x.emp.fechaInicio, now))
                        });

            }
            //Lista con datos recibidos de la consulta
            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.nombre + " " + dato.cantDesa + " " + dato.promedio);
            }

            return View(datos);//retorna la vista
        }

        // Método para consulta de todos los requerimientos
        /*
         * @return retorna una lista
         */
        public ActionResult EstadoRequerimiento()
        {

            ViewBag.Proy = "Todos los Proyectos";
            var item = (from a in db.Requerimiento
                    from b in db.EmpleadoDesarrollador
                    where a.cedulaResponsable_FK == b.cedulaED
                    select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });
            //Lista con datos recibidos de la consulta
            List<string> datosObtenidos = new List<string>();
            foreach (var dato in item)
            {
                datosObtenidos.Add(dato.nombreReq + " " + dato.estadoReq + " " + dato.nombreDes + " " + dato.apellido1Des + " " + dato.apellido2Des);
            }

            return View(datosObtenidos);//retorna la vista

        }

        [HttpPost]
        // Método de consulta por requerimiento
        /*
         * @return lista con datos obtenidos
         */
        public ActionResult EstadoRequerimiento(string Proyecto, string ordenado)
        {
            //Como la consulta es solo para clientes se despliega solo los proyectos de los que son dueños
            if (usuario == "Cliente")
            { //si soy cliente puedo solamente ver  mis proyectos
                var obj = from a in db.Proyecto
                          where a.cedulaCliente == cedula
                          select a;

                return View(obj.Distinct().ToList());
            }

            ViewBag.Proy = Proyecto;//Saber cuál proyecto eligió y mostrarselo en vista

            if (ordenado == "" || ordenado == "Ordenado por estado")//Saber que orden escogió
            {   //Y realiza la consulta para ese proyecto con ese orden
                var item = (from a in db.Requerimiento
                            from b in db.EmpleadoDesarrollador
                            where a.cedulaResponsable_FK == b.cedulaED
                            where a.nombreProyecto_FK == Proyecto
                            orderby a.estado ascending
                            select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });
                //Consulta si quiere ver requerimientos de todos los proyectos
                if (Proyecto == "" || Proyecto == "Todos los Proyectos")
                {
                    ViewBag.Proy = "Todos los Proyectos";
                    item = (from a in db.Requerimiento
                            from b in db.EmpleadoDesarrollador
                            where a.cedulaResponsable_FK == b.cedulaED
                            orderby a.estado ascending
                            select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });

                }
                //Lista con datos recibidos de la consulta
                List<string> datosObtenidos = new List<string>();
                foreach (var dato in item)
                {
                    datosObtenidos.Add(dato.nombreReq + " " + dato.estadoReq + " " + dato.nombreDes + " " + dato.apellido1Des + " " + dato.apellido2Des);
                }

                return View(datosObtenidos);//retorna la vista
            }
            else
            {//Y realiza la consulta con el otro orden
                var item = (from a in db.Requerimiento
                            from b in db.EmpleadoDesarrollador
                            where a.cedulaResponsable_FK == b.cedulaED
                            where a.nombreProyecto_FK == Proyecto
                            orderby b.nombreED, b.apellido1ED ascending
                            select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });
                //Consulta si quiere ver requerimientos de todos los proyectos
                if (Proyecto == "" || Proyecto == "Todos los Proyectos")
                {
                    ViewBag.Proy = "Todos los Proyectos";
                    item = (from a in db.Requerimiento
                            from b in db.EmpleadoDesarrollador
                            where a.cedulaResponsable_FK == b.cedulaED
                            orderby b.nombreED, b.apellido1ED ascending
                            select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });

                }
                //Lista con datos recibidos de la consulta
                List<string> datosObtenidos = new List<string>();
                foreach (var dato in item)
                {
                    datosObtenidos.Add(dato.nombreReq + " " + dato.estadoReq + " " + dato.nombreDes + " " + dato.apellido1Des + " " + dato.apellido2Des);
                }
                return View(datosObtenidos);//retorna la vista
            }

        }

        // Nombres para el dropdown de ordenamiento
        /*
         * @return  lista de las opciones de ordenamiento
         */
        public List<SelectListItem> getOrdenamiento()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "Ordenado por estado" });
            items.Add(new SelectListItem() { Text = "Ordenado por responsable" });
            return items;

        }

        // Obtine todos los proyectos del sistema
        /*
         * @return  lista de proyectos
         */
        public SelectList getProyectos(String rol, String cedula)
        {
            return proyController.getProyectos(rol, cedula);

        }

        // Obtiene las habilidades del sistema
        /*
         * @return  lista de habilidades
         */
        public SelectList getHabilidades()
        {
            return habController.getHabilidades();
        }
    }


}

