using System;

namespace ApartmanYonetimSistemi.Models
{
    public class ArizaTalebi
    {
        public int Id { get; set; }
        public int DaireNo { get; set; }

        public string? SakinAd{ get; set; }

        public string? Aciklama { get; set; }
        public string? Baslik { get; set; } 
        public string? Durum { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}