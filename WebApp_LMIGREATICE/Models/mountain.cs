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

    public partial class mountain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public mountain()
        {
            this.glaciers = new HashSet<glacier>();
        }

        public int idMountain { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [RegularExpression("[A-Z]+", ErrorMessage = "Must be written in capital letters. It can not contain numbers, spaces or special characters")]
        [MaxLength(25, ErrorMessage = "Must be between 1 and 25 characters")]
        [DisplayName("Name Mountain")]
        public string nameMountain { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [DisplayName("Country")]
        public int idProjectCountry { get; set; }

        [Required(ErrorMessage = "Field Required")]
        [DisplayName("State")]
        public bool stateR { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<glacier> glaciers { get; set; }
        public virtual projectCountry projectCountry { get; set; }
    }
}