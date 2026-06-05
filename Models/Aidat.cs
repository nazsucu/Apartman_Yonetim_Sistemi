using System;

namespace ApartmanYonetimSistemi.Models
{
    public class Aidat
    {
        public int Id { get; set; }
        public decimal Tutar { get; set; }
        public required string Ay { get; set; } 
        public bool OdendiMi { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public required string SakinAdSoyad { get; set; } 
    }
}