using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models
{
    public class Yonetici
    {
        [Key]
        public int Id { get; set; }
        public required string AdSoyad { get; set; }
        public required string KullaniciAdi { get; set; }
        public required string Sifre { get; set; }

        public string? ApartmanAdi { get; set; }
        public string? YonetimIban { get; set; }
    }
}