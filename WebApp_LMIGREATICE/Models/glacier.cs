//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApp_LMIGREATICE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class glacier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public glacier()
        {
            this.stations = new HashSet<station>();
        }

        public int idGlacier { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [MaxLength(30, ErrorMessage = "Must be between 1 and 30 characters")]
        [DisplayName("Name Glacier")]
        public string nameGlacier { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [DisplayName("Mountain")]
        public int idMountain { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [DisplayName("State")]
        public bool stateR { get; set; }
        public virtual mountain mountain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<station> stations { get; set; }
    }
}
