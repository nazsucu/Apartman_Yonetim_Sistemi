using System;
using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models
{
    public class Duyuru
    {
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public string Icerik { get; set; } = string.Empty;

        public DateTime YayinTarihi { get; set; } = DateTime.Now;
    }
}