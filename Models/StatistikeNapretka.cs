using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiShape.Models
{
    public class StatistikeNapretka
    {
        [Key]
        public int IdZapisa { get; set; }
        public DateTime Datum { get; set; }
        public double Tezina { get; set; }
        public double Bmi { get; set; }
        public int KalorijskiUnos { get; set; }

        [ForeignKey("Korisnik")]
        public int IdKorisnika { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}
