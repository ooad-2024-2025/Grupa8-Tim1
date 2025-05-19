using System.ComponentModel.DataAnnotations;

namespace OptiShape.Models
{
    public class Korisnik
    {
        [Key]
        public int IdKorisnika { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Sifra { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public double Visina { get; set; }
        public double Tezina { get; set; }
        public Spol Spol { get; set; }
        public Cilj Cilj { get; set; }
        public bool StudentskiStatus { get; set; }
        public string BrojTelefona { get; set; }
    }
}
