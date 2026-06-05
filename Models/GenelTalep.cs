using System;

namespace ApartmanYonetimSistemi.Models
{
    public class GenelTalep
    {
        public int Id { get; set; }

        public string? YoneticiCevap { get; set; } 
        public string? SakinAd { get; set; }
        public string? Konu { get; set; }

        public string? Mesaj { get; set; }
        public DateTime Tarih { get; set; }
        public string? Durum { get; set; }

        public bool OkunduMu { get; set; }
    }
}