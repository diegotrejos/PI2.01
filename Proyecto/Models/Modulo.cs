
namespace Proyecto.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Modulo // porque partial? es por la relacion???
    {
        //Data notation 
        //Sirven para hacer validaciones o dar formato a las variables

        [Key] // Creo que se refiere a PK o FK
        [StringLength(15)] //Constraint que limita los strings 
        [Display(Name = "Nombre de Proyecto")]
        public string NombreProy { get; set; }

        [Display(Name = "Identificador")]
        [Required(ErrorMessage = "Debe tener identificador")]
        public int Id { get; set; }


        [Required(ErrorMessage = "Debe tener Nombre")] //Constraint es necesario
        [Display(Name = "Nombre de Modulo")]
        [StringLength(15)]
        public string Nombre { get; set; }

        public virtual Proyecto Proyecto { get; set; } //FK?

    }
}