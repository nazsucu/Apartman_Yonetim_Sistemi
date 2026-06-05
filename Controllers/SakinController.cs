using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Data;
using ApartmanYonetimSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmanYonetimSistemi.Controllers
{
    public class SakinController : Controller
    {
        private readonly AppDbContext _context;

        public SakinController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string arama, string sirala)
        {
           
            var liste = _context.Sakinler.ToList();

            
            if (!string.IsNullOrEmpty(arama))
            {
                liste = liste.Where(s =>
                    (s.AdSoyad != null && s.AdSoyad.Contains(arama, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Email != null && s.Email.Contains(arama, StringComparison.OrdinalIgnoreCase)) ||
                    s.DaireNo.ToString().Contains(arama)
                ).ToList();
            }
            liste = sirala switch
            {
                "ad_asc" => liste.OrderBy(s => s.AdSoyad).ToList(),
                "ad_desc" => liste.OrderByDescending(s => s.AdSoyad).ToList(),
                "daire" => liste.OrderBy(s => s.DaireNo).ToList(),
                _ => liste.OrderBy(s => s.AdSoyad).ToList()
            };

            ViewBag.Arama = arama;
            return View(liste);
        }

        [HttpPost]
        public IActionResult Ekle(Sakin yeniSakin)
        {
            
            if (yeniSakin != null)
            {
                if (yeniSakin.DaireNo <= 0)
                {
                    yeniSakin.DaireNo = 1;
                }

                string temizAd = yeniSakin.AdSoyad.ToLower()
                    .Replace(" ", "")
                    .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                    .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

                yeniSakin.KullaniciAdi = temizAd + yeniSakin.DaireNo;

                yeniSakin.Sifre = "1234";

                _context.Sakinler.Add(yeniSakin);

                _context.SaveChanges();
                TempData["Mesaj"] = $"Sakin başarıyla eklendi! <br/><b>Kullanıcı Adı:</b> {yeniSakin.KullaniciAdi} <br/><b>Şifre:</b> {yeniSakin.Sifre}";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Sil(int id)
        {
            var sakin = _context.Sakinler.Find(id);

            if (sakin != null)
            {
                var aidatlar = _context.Aidatlar.Where(a => a.SakinAdSoyad == sakin.AdSoyad).ToList();
                _context.Aidatlar.RemoveRange(aidatlar);
                var genelTalepler = _context.GenelTalepler.Where(x => x.SakinAd == sakin.AdSoyad).ToList();
                _context.GenelTalepler.RemoveRange(genelTalepler);
                _context.Sakinler.Remove(sakin);
                _context.SaveChanges();

                TempData["Mesaj"] = "Sakine ait her şey temizlendi!";
            }
            else
            {
                TempData["Mesaj"] = "Hata: Silinmek istenen sakin bulunamadı!";
            }
            return RedirectToAction("Index");
        }
    }
}