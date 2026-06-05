using System;
using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models
{
    public class Harcama
    {
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        public decimal Tutar { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        public string Kategori { get; set; } = "Genel";
    }
}