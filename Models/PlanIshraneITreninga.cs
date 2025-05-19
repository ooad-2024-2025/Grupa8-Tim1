using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiShape.Models
{
    public class PlanIshraneTreninga
    {
        [Key]
        public int IdPlana { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int Kalorije { get; set; }
        public int Protein { get; set; }
        public int Ugljikohidrati { get; set; }
        public int Masti { get; set; }

        [ForeignKey("Korisnik")]
        public int IdKorisnika { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}
