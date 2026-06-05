using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Models;
using System.Collections.Generic;
using System.Linq;
using ApartmanYonetimSistemi.Data;
using System;
using Microsoft.AspNetCore.Http;

namespace ApartmanYonetimSistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult YoneticiLogin(string KullaniciAdi, string Sifre)
        {
            var admin = _context.Yoneticiler.FirstOrDefault(u => u.KullaniciAdi == KullaniciAdi);
            if (admin != null && admin.Sifre.Trim() == Sifre.Trim())
            {
                HttpContext.Session.SetString("YoneticiAd", admin.AdSoyad);
                return RedirectToAction("Dashboard", "Admin");
            }
            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View("Index");
        }

        public IActionResult CikisYap()
        {
            HttpContext.Session.Remove("YoneticiAd");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Dashboard()
        {
            ViewBag.ToplamDaire = _context.Sakinler.Count();
            ViewBag.BekleyenTalep = _context.GenelTalepler.Count(t => !t.OkunduMu);

            var toplamKasa = _context.Aidatlar.Where(a => a.OdendiMi).Sum(a => (decimal?)a.Tutar) ?? 0;
            ViewBag.Kasa = toplamKasa.ToString("N2");

            var toplamAidatSayisi = _context.Aidatlar.Count();
            if (toplamAidatSayisi > 0)
            {
                var odenenAidatSayisi = _context.Aidatlar.Count(a => a.OdendiMi);
                double oran = (double)odenenAidatSayisi / toplamAidatSayisi * 100;
                ViewBag.AidatYuzde = Math.Round(oran);
            }
            else
            {
                ViewBag.AidatYuzde = 0;
            }

            var aktifSakinIsimleri = _context.Sakinler.Select(s => s.AdSoyad).ToHashSet();

            var sonTalepler = _context.GenelTalepler
                                       .AsEnumerable() 
                                       .Where(t => aktifSakinIsimleri.Contains(t.SakinAd)) 
                                       .OrderByDescending(t => t.Tarih) 
                                       .ToList();

            return View("Dashboard", sonTalepler);
        }

        public IActionResult Arizalar()
        {
            var talepler = _context.Arizalar.ToList();
            return View("Arizalar", talepler);
        }

        public IActionResult Ayarlar()
        {
            ViewBag.TumYoneticiler = _context.Yoneticiler.ToList();
            var admin = _context.Yoneticiler.FirstOrDefault();
            return View("Ayarlar", admin);
        }

        [HttpPost]
        public IActionResult AyarlariGuncelle(string apartmanAdi, string yonetimIban)
        {
            var admin = _context.Yoneticiler.FirstOrDefault();
            if (admin != null)
            {
                admin.ApartmanAdi = apartmanAdi;
                admin.YonetimIban = yonetimIban;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Bilgiler başarıyla güncellendi.";
            }
            return RedirectToAction("Ayarlar");
        }

        [HttpPost]
        public IActionResult SifreGuncelle(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var admin = _context.Yoneticiler.FirstOrDefault();
            if (admin == null) return RedirectToAction("Ayarlar");

            string girilenMevcut = mevcutSifre?.Trim() ?? "";

            if (admin.Sifre.Trim() != girilenMevcut)
            {
                TempData["Hata"] = "Mevcut şifreniz hatalı!";
                return RedirectToAction("Ayarlar");
            }

            if (string.IsNullOrEmpty(yeniSifre) || yeniSifre != yeniSifreTekrar)
            {
                TempData["Hata"] = "Yeni şifreler eşleşmiyor!";
                return RedirectToAction("Ayarlar");
            }

            admin.Sifre = yeniSifre.Trim();
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Şifreniz başarıyla değişti!";
            return RedirectToAction("Ayarlar");
        }

        [HttpPost]
        public IActionResult YoneticiEkle(Yonetici yeniYonetici)
        {
            if (yeniYonetici != null)
            {
                yeniYonetici.Sifre = yeniYonetici.Sifre?.Trim();
                _context.Yoneticiler.Add(yeniYonetici);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Yeni yönetici başarıyla eklendi.";
            }
            return RedirectToAction("Ayarlar");
        }

        [HttpPost]
        public IActionResult YoneticiSil(int id)
        {
            var admin = _context.Yoneticiler.Find(id);
            var toplamAdmin = _context.Yoneticiler.Count();

            if (admin != null && toplamAdmin > 1)
            {
                _context.Yoneticiler.Remove(admin);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Yönetici silindi.";
            }
            else
            {
                TempData["Hata"] = "Sistemde en az bir yönetici bulunmalıdır!";
            }
            return RedirectToAction("Ayarlar");
        }

        [HttpPost]
        public IActionResult ArizaCoz(int id)
        {
            var ariza = _context.Arizalar.FirstOrDefault(x => x.Id == id);
            if (ariza != null)
            {
                ariza.Durum = "Tamamlandı";
                _context.SaveChanges();
            }
            return RedirectToAction("Arizalar");
        }

        [HttpPost]
        public IActionResult ArizaSil(int id)
        {
            var ariza = _context.Arizalar.FirstOrDefault(x => x.Id == id);
            if (ariza != null)
            {
                _context.Arizalar.Remove(ariza);
                _context.SaveChanges();
            }
            return RedirectToAction("Arizalar");
        }

        [HttpPost]
        public IActionResult TalepYanitla(int id, string cevap)
        {
            var talep = _context.GenelTalepler.FirstOrDefault(x => x.Id == id);
            if (talep != null)
            {
                talep.YoneticiCevap = cevap;
                talep.Durum = "Yanıtlandı";
                talep.OkunduMu = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}