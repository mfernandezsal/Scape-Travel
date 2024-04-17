using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScapeTravelWEB.Models.ScapeTravelDB
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CI { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido_pat { get; set; }

        [Required]
        public string Apellido_mat { get; set; }

        [Required]
        public DateTime Nacimiento { get; set; }

        [Required]
        public string Genero { get; set; }

        [Required]
        public string Nacionalidad { get; set; }

        [Required]
        public string Nro_pasaporte { get; set; }

        public string Telefono { get; set; }

        public int? FamiliaId { get; set; }

        public Familia Familia { get; set; }

    }
}