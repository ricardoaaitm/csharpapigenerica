
using System.ComponentModel.DataAnnotations;

namespace Csharpapigenerica.Models
{
    public class IndicadorDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        public string Objetivo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Alcance { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Formula { get; set; } = string.Empty;

        [Required]
        public int FkidTipoIndicador { get; set; }

        [Required]
        public int FkidUnidadMedicion { get; set; }

        [Required]
        [StringLength(1000)]
        public string Meta { get; set; } = string.Empty;

        [Required]
        public int FkidSentido { get; set; }

        [Required]
        public int FkidFrecuencia { get; set; }

        public string? FkidArticulo { get; set; }
        public string? Fkidliteral { get; set; }
        public string? FkidNumeral { get; set; }
        public string? FkidParagrafo { get; set; }
    }
}
