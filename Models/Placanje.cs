using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiShape.Models
{
    public class Placanje
    {
        [Key]
        public int IdPlacanja { get; set; }
        public DateTime DatumPlacanja { get; set; }
        public double Iznos { get; set; }
        public bool PopustPrimijenjen { get; set; }
        public NacinPlacanja NacinPlacanja { get; set; }

        [ForeignKey("Korisnik")]
        public int IdKorisnika { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}
