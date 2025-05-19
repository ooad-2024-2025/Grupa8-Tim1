using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiShape.Models
{
    public class Termin
    {
        [Key]
        public int IdTermina { get; set; }
        public DateTime Datum { get; set; }

        [ForeignKey("Korisnik")]
        public int IdKorisnika { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}
