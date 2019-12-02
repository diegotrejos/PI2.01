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
        /*Vista inicial para reportería
         * @return view()
         */
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;//Saber rol del que está en el sistema
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;//Proyecto del que está en el sistema
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;//Cédula del que está en el sistema
            ViewBag.user = usuario;
            return View();
        }

        /*Método inicial para la vista de la consulta sobre la comparacion real y estimada de los requerimientos
         * @return view(lista): retorna a la vista una lista vacía (Katherine)
         */
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


        /*Método para vista parcial que retorna la informacion necesaria para la consulta de comparacion entre la duración estimada 
         * y real de los requerimientos para un desarrollador y proyecto específico
         * @param nombre: nombre de desarrollador
         * @param nombreProyecto: nombre del proyecto del desarrollador
         * @return view(lista): lista con información de la consulta
         * (Katherine)
         */
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

        /*Método inicial que responde a la consulta de horas estimadas y reales necesarias para terminar un proyecto
         * se muestran inicialmente todos los proyectos en esta primera vista
         * @return view(lista): lista con la información de todos los proyectos
         * (Katherine)
         */
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
                           select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED +" "+ c.FirstOrDefault().l.apellido1ED +" "+ c.FirstOrDefault().l.apellido2ED};
            if (usuario == "Lider") {
                item =
                         from b in db.Requerimiento
                         from a in db.Proyecto
                         from e in db.Equipo
                         from l in db.EmpleadoDesarrollador
                         where b.nombreProyecto_FK == a.nombre  && e.cedulaEM_FK == cedula && e.rol == true && e.nombreProy_FK == a.nombre  &&  a.fechaFinalizacion != null
                         group new { a, b, e, l } by a.nombre into c
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED +" "+ c.FirstOrDefault().l.apellido1ED+" "+ c.FirstOrDefault().l.apellido2ED };

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

        /*Método post que responde a la consulta de horas estimadas y reales para concluir con un proyecto específico
         * @Param nombreProyecto
         * @return view(lista): lista con la información del proyecto específico
         * (Katherine)
         */
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
                           select new { nombre = c.FirstOrDefault().a.nombre, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED +" "+ c.FirstOrDefault().l.apellido1ED+" "+ c.FirstOrDefault().l.apellido2ED};
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
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED + " " + c.FirstOrDefault().l.apellido1ED + " " + c.FirstOrDefault().l.apellido2ED };
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


        /*Método que retorna el nommbre de los desarrolladores
         * @return desarrolladores (katherine)
         */
        public List<SelectListItem> getDesarrolladores()
        {
   
            return emplController.getDesarrolladores();
        }

        /*Método que retorna los proyectos del desarrollador en formato Json
         * esto para llenar el segundo dropdown por medio de Jquery
         * al seleccionar el primer dropdown de la vista TiemposRequerimientos
         * @param nombre: nombre del desarrollador
         * (Katherine)
         */
        [HttpPost]
        public JsonResult getProyectoDesarrollador(string nombre)
        {

           
            System.Diagnostics.Debug.WriteLine("el nombre para json: " + nombre);
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

        /*Metodo que retorna los proyectos según el rol loggeado
         *  @return proyectos
         */
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
           string total;

        }


        //listo
        public ActionResult PeriodosDisponibles()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;
            List<Desocupacion> Empleados = new List<Desocupacion>();

            Desocupacion p = new Desocupacion();
            p.nombreED = "------------";
            p.periodosyDias = "--";
            p.total = "---";
            //los agrego a la lista 
            Empleados.Add(p);
            ViewBag.fInicio = "ninguna";
            ViewBag.ffinal = "ninguna";
            return View(Empleados);

        }

        [HttpPost]
        public ActionResult PeriodosDisponibles(DateTime Finicio, DateTime Ffinal)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;
            ViewBag.fInicio = Finicio;
            ViewBag.ffinal = Ffinal;
            ViewBag.lleno = true;
            //genere lista de objetos para mostrar luego en vista
            List<Desocupacion> Empleados = new List<Desocupacion>();

            //codigo sql ultilizando cursores
            string sql = @"
					  DECLARE
		
                        @inicio date,--limite inferior iterador
                        @final date,--limite superior iterador
                        @Viejoinicio date,--limite inferior iterador
                        @Viejofinal date,--limite superior iterador
                        @diasLib int, --dias libres de iteradores
                        @Mensaje varchar(100)--variable temporal para formar mensajes a imprimir
			
		            Declare	@cursorResult table (mensaje varchar(100))

                        DECLARE DispCur CURSOR FOR
                        SELECT  DISTINCT P.fechaInicio,P.fechaFinalizacion
                        FROM Equipo Eq
                        JOIN Proyecto P
                        ON P.nombre =Eq.nombreProy_FK
                        JOIN EmpleadoDesarrollador E
                        ON E.cedulaED = Eq.cedulaEM_FK
                        WHERE Eq.cedulaEM_FK = @empleado
                        AND (0 <= DATEDIFF(Day,@fechainicio,P.fechaInicio)AND 0 >= DATEDIFF(Day,@fechafin,P.fechaFinalizacion)) --CASO QUE esta entre limites
                        OR (0 >= DATEDIFF(Day,@fechainicio,P.fechaInicio)AND 0 >= DATEDIFF(Day,@fechafin,P.fechaFinalizacion)AND 0 < DATEDIFF(Day,@fechainicio,P.fechaFinalizacion))--limite inferior en medio de proyecto
                        OR  (0 <= DATEDIFF(Day,@fechainicio,P.fechaInicio)AND 0 <= DATEDIFF(Day,@fechafin,P.fechaFinalizacion)AND 0 >= DATEDIFF(Day,@fechafin,P.fechaInicio))--limite superior en medio de proyecto
                        ORDER BY P.fechaInicio


                        OPEN DispCur
                        SET @diasLib = 0
						SET @Viejoinicio =(SELECT E.fechaInicio FROM EmpleadoDesarrollador E WHERE E.cedulaED = @empleado)
	                        FETCH NEXT FROM DispCur INTO @inicio,@final
	                         IF @inicio > @fechainicio--en caso de que proyecto aun no haya iniciado en limite inferior
		                        BEGIN
								IF @fechainicio < @Viejoinicio --en caso de	q empleado aun no haya empezado a trabajar
									BEGIN
									SET @Mensaje = CAST(@Viejoinicio AS nvarchar(10)) +' -- '+CAST(@inicio AS nvarchar(10)) +':  '+ CAST(DATEDIFF(Day, @Viejoinicio,@inicio) AS nvarchar (100))
									SET @diasLib = @diasLib+(DATEDIFF(Day, @Viejoinicio,@inicio))
                                    insert into @cursorResult(mensaje) values(@Mensaje)
									END
								ELSE
									BEGIN
									SET @Mensaje = '  INICIO ' +' -- '+CAST(@inicio AS nvarchar(10)) +':  '+ CAST(DATEDIFF(Day, @fechainicio,@inicio) AS nvarchar (100))
									SET @diasLib = @diasLib+(DATEDIFF(Day, @fechainicio,@inicio))
									insert into @cursorResult(mensaje) values(@Mensaje)
									END
								END
	                        WHILE @@fetch_status = 0
	                        BEGIN
		                        SET @Viejoinicio = @inicio
		                        SET @Viejofinal = @final
		                        FETCH NEXT FROM DispCur INTO @inicio,@final
		                        IF @Viejoinicio = @inicio AND @final < @fechafin--si proyecto termina antes de limite superior
			                        BEGIN
			                        SET @Mensaje = CAST(@Viejofinal AS nvarchar(10)) +' -- '+' FINAL:  '+'  '+ CAST(DATEDIFF(Day,@Viejofinal, @fechafin) AS nvarchar (10))
			                        SET @diasLib = @diasLib+(DATEDIFF(Day,@Viejofinal, @fechafin))
			                            insert into @cursorResult(mensaje) values(@Mensaje)
			                        END
		                        ELSE IF @final < @fechafin--ciclo normal 
			                        BEGIN
			                        SET @Mensaje = CAST(@Viejofinal AS nvarchar(10)) +' -- '+CAST(@inicio AS nvarchar(10))+':  '+ CAST(DATEDIFF(Day,@Viejofinal, @inicio) AS nvarchar (10))
			                        SET @diasLib = @diasLib+(DATEDIFF(Day,@Viejofinal, @inicio)) 
			                        insert into @cursorResult(mensaje) values(@Mensaje)
			                        END	
		                        ELSE IF @Viejoinicio != @inicio AND @final > @fechafin--si proyecto termina despues de limite superior
			                        BEGIN
			                        SET @Mensaje = CAST(@Viejofinal AS nvarchar(10)) +' -- '+CAST(@fechafin AS nvarchar(10))+':  '+ CAST(DATEDIFF(Day,@Viejofinal, @fechafin) AS nvarchar (10))
			                        SET @diasLib = @diasLib + (DATEDIFF(Day,@Viejofinal, @fechafin))
			                        insert into @cursorResult(mensaje) values(@Mensaje)
			                        END
	                        END
                                SET @Mensaje = CAST(@diaslib AS nvarchar(10))
		                        insert into @cursorResult(mensaje) values(@Mensaje)
                        CLOSE DispCur
                        DEALLOCATE DispCur;
			            select * from @cursorResult
            ";

            //genero listas de personas
            var personas = from emp in db.EmpleadoDesarrollador
                           select emp;




            foreach (var item in personas.ToList())
            {
                //genero parametros para sql dinamico
                var fechainicio = new SqlParameter("@fechainicio", Finicio.Date);
                var fechafin = new SqlParameter("@fechafin", Ffinal.Date);
                var emp_actual = new SqlParameter("@empleado", item.cedulaED);
                var results = db.Database.SqlQuery<string>(sql, fechainicio, fechafin, emp_actual).ToList();
                string per = item.nombreED;
                //objeto que metere en lista
                Desocupacion p = new Desocupacion();

                p.nombreED = item.nombreED;


                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                foreach (var linea in results)
                {

                    if (linea != results.LastOrDefault())
                    {
                        p.periodosyDias += linea.ToString();
                        p.periodosyDias += System.Environment.NewLine;
                    }
                }
                p.total = results.LastOrDefault();


                //los agrego a la lista SI tienen datos
                if (Convert.ToInt32(p.total) > 0)
                {
                    Empleados.Add(p);
                }
            }
            //caso de que no existan empleados en el intervalo
            if (Empleados.Count() == 0)
            {
                ViewBag.lleno = false;
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
                      select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = Math.Round(g.Average(c => c.duracionReal),2) };

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
                       select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = Math.Round(g.Average(c =>c.duracionReal),2) };

            //Devuelve la información de todos los niveles
            if (complejidad == "" || complejidad == "Todos los niveles")
            {
                item = from req in db.Requerimiento
                       from proy in db.Proyecto
                       where proy.nombre == req.nombreProyecto_FK
                       where proy.fechaFinalizacion != null
                       group req by 1 into g
                       select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = Math.Round(g.Average(c => c.duracionReal),2) };
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
	
	 public ActionResult DesarrolladoresProyDis()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            
            TempData["empleadosDisponibles"] = empleados;
            List<Proyecto.Models.ViewModels.infoEmpleados> lista_datos = new List<Proyecto.Models.ViewModels.infoEmpleados>();

            int longitudArray = new EmpleadoDesarrolladorController().getNoDisponibles();


            //info_empleados[] arrayInfo = new info_empleados[longitudArray];

            //arrayInfo[1].longitud = longitudArray;

            new EquipoController().llenarArray(lista_datos);

            new RequerimientoController().llenarArray(lista_datos); //importante aqui la orden, primero se tiene que que llenar la array en equipo controller

            calculoDurEstimada(lista_datos);

            ordenarListaEmpleados(lista_datos);

            //ordenamientoElka(lista_datos,longitudArray);

            TempData["datos"] = lista_datos;
            return View();
        }

        public ActionResult TotalReqTerminadosEnEjecucion()
        {

            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            //List<Proyecto.Models.ViewModels.TotalReqPorCliente> lista_totalReq = new List<Proyecto.Models.ViewModels.TotalReqPorCliente>();


            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().GetProyectosDeCliente(cedula);

            int index = 0;

            foreach (var item in proyectos)
            {
                lista_totalReq.Add(new Proyecto.Models.ViewModels.TotalReqPorCliente());
                lista_totalReq.Add(new Proyecto.Models.ViewModels.TotalReqPorCliente());
                //para los requerimientos en ejecucion
                lista_totalReq[index].nombreProy = item.nombre;
                lista_totalReq[index].durEstimada = (DateTime)item.fechaInicio;
                lista_totalReq[index].nombreCliente = item.Cliente.nombre;
                lista_totalReq[index].apellidoCliente = item.Cliente.apellido1;
                //para los requerimientos finalizados
                lista_totalReq[index + 1].nombreProy = item.nombre;
                lista_totalReq[index + 1].durEstimada = (DateTime)item.fechaInicio;
                lista_totalReq[index + 1].nombreCliente = item.Cliente.nombre;
                lista_totalReq[index + 1].apellidoCliente = item.Cliente.apellido1;
                new RequerimientoController().llenarListaReq(lista_totalReq, item.nombre,index);
                index += 2;
            }

            TempData["Lista"] = lista_totalReq;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalReqTerminadosEnEjecucion(string filtro)
        {
            int index = 0;
            List<Proyecto.Models.ViewModels.TotalReqPorCliente> lista_totalReqFiltrada = new List<Proyecto.Models.ViewModels.TotalReqPorCliente>();
            foreach (var item in lista_totalReq)
            {
                if (item.nombreProy == filtro)
                {
                    lista_totalReqFiltrada.Add(new Models.ViewModels.TotalReqPorCliente());
                    lista_totalReqFiltrada[index++] = item;
                }
            }
            TempData["Lista"] = lista_totalReqFiltrada;
            return View();
        }

            private void calculoDurEstimada(List<Proyecto.Models.ViewModels.infoEmpleados> info)
        {
            foreach (var item1 in info)
            {
                double totalHoras = 0;
                foreach (var item in item1.requerimientos)
                {
                    totalHoras += item.duracionEstimada;
                }
                totalHoras = totalHoras / 8;
                item1.durEstimada = (DateTime)item1.equipo.Proyecto.fechaInicio;
                item1.durEstimada = item1.durEstimada.AddDays(totalHoras);
              
            }
        }

        private void ordenarListaEmpleados(List<Proyecto.Models.ViewModels.infoEmpleados> info)
        {
            int elementos = 0; 
            foreach (var item in info)
            {
                ++elementos;
            }
            int marca = 0;
            int comparador = 0;
            Proyecto.Models.ViewModels.infoEmpleados aux;
            int k; 
            for (int i = 0; i < elementos; ++i)
            {
                for (k = elementos - 1; k > marca; --k)
                {
                    comparador = DateTime.Compare(info[k - 1].durEstimada, info[k].durEstimada);
                    if (comparador > 0)
                    {
                        aux = info[k - 1];
                        info[k - 1] = info[k];
                        info[k] = aux;
                    }
                }
                ++marca;
            }
        }
        
    }


}

