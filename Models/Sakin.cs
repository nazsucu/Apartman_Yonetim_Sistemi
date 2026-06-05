using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models
{
    public class Sakin
    {
        [Key] 
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Daire No zorunludur.")]
        public int DaireNo { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        public string? Telefon { get; set; }

       
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }

    }
}