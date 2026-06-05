
using ApartmanYonetimSistemi.Data;
using ApartmanYonetimSistemi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApartmanYonetimSistemi.Controllers
{
    public class SakinPanelController : Controller
    {
        private readonly AppDbContext _context;

        public SakinPanelController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            ViewBag.ToplamBorc = _context.Aidatlar
                .Where(a => a.SakinAdSoyad == sakinAd && !a.OdendiMi)
                .Sum(a => (decimal?)a.Tutar) ?? 0;

            var bugun = DateTime.Today;
            var gunlukDuyurular = _context.Duyurular
                .Where(d => d.YayinTarihi.Date == bugun)
                .OrderByDescending(d => d.YayinTarihi)
                .ToList();

            return View(gunlukDuyurular);
        }

        public IActionResult Duyurular(string sure = "hepsi", string sirala = "yeni")
        {
            var query = _context.Duyurular.AsQueryable();
            var bugun = DateTime.Today;

            switch (sure)
            {
                case "son7": query = query.Where(d => d.YayinTarihi >= bugun.AddDays(-7)); break;
                case "son30": query = query.Where(d => d.YayinTarihi >= bugun.AddMonths(-1)); break;
            }

            query = sirala switch
            {
                "eski" => query.OrderBy(d => d.YayinTarihi),
                "az" => query.OrderBy(d => d.Baslik),
                "za" => query.OrderByDescending(d => d.Baslik),
                _ => query.OrderByDescending(d => d.YayinTarihi)
            };

            ViewBag.SeciliFiltre = sure;
            ViewBag.SeciliSirala = sirala;

            return View(query.ToList());
        }

        public IActionResult Aidatlarim()
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var aidatlar = _context.Aidatlar
                .Where(a => a.SakinAdSoyad == sakinAd)
                .OrderByDescending(a => a.KayitTarihi)
                .ToList();

            return View(aidatlar);
        }

        public IActionResult Arizalarim()
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var arizalar = _context.Arizalar
                .Where(x => x.SakinAd == sakinAd)
                .OrderByDescending(x => x.Tarih)
                .ToList();
            return View(arizalar);
        }

        public IActionResult Taleplerim()
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var talepler = _context.GenelTalepler
                .Where(x => x.SakinAd == sakinAd)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(talepler);
        }

        [HttpPost]
        public IActionResult TalepKaydet(string baslik, string kategori, string aciklama)
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var gercekSakin = _context.Sakinler.FirstOrDefault(s => s.AdSoyad == sakinAd);

            int daireNo = gercekSakin != null ? gercekSakin.DaireNo : 0;
            if (kategori == "Arıza")
            {
                var yeniAriza = new ArizaTalebi
                {
                    SakinAd = sakinAd,
                    DaireNo = daireNo, 
                    Baslik = baslik,
                    Aciklama = aciklama,
                    Tarih = DateTime.Now,
                    Durum = "Beklemede" 
                };
                _context.Arizalar.Add(yeniAriza);
            }
            else
            {
                var yeniTalep = new GenelTalep
                {
                    SakinAd = sakinAd,
                    Konu = baslik,
                    Mesaj = aciklama,
                    Tarih = DateTime.Now,
                    Durum = "Beklemede",
                    OkunduMu = false
                };
                _context.GenelTalepler.Add(yeniTalep);
            }

            _context.SaveChanges();

            return RedirectToAction(kategori == "Arıza" ? "Arizalarim" : "Taleplerim");
        }

        [HttpGet]
        public IActionResult Ayarlar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SifreGuncelle(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var sakinIdRaw = HttpContext.Session.GetInt32("SakinId");
            int sakinId = 0;

            if (sakinIdRaw != null)
            {
                sakinId = sakinIdRaw.Value;
            }
            else
            {
                var sakinIdStr = HttpContext.Session.GetString("SakinId");
                if (string.IsNullOrEmpty(sakinIdStr) || !int.TryParse(sakinIdStr, out sakinId))
                {
                    return RedirectToAction("Login", "Home");
                }
            }

            var sakin = _context.Sakinler.FirstOrDefault(x => x.Id == sakinId);

            if (sakin == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Ayarlar");
            }

            if (sakin.Sifre != mevcutSifre)
            {
                TempData["ErrorMessage"] = "Mevcut şifrenizi hatalı girdiniz!";
                return RedirectToAction("Ayarlar");
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["ErrorMessage"] = "Yeni şifreler birbiriyle uyuşmuyor!";
                return RedirectToAction("Ayarlar");
            }

            if (yeniSifre.Length < 4)
            {
                TempData["ErrorMessage"] = "Yeni şifre en az 4 karakter olmalıdır!";
                return RedirectToAction("Ayarlar");
            }

            sakin.Sifre = yeniSifre;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Ayarlar");
        }

        [HttpGet]
        public IActionResult OdemeYap(int aidatId)
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var aidat = _context.Aidatlar.FirstOrDefault(a => a.Id == aidatId && a.SakinAdSoyad == sakinAd);
            if (aidat == null)
            {
                TempData["ErrorMessage"] = "Aidat bilgisi bulunamadı veya size ait değil!";
                return RedirectToAction("Aidatlarim");
            }

            ViewBag.AidatId = aidat.Id;
            ViewBag.Tutar = aidat.Tutar;
            ViewBag.Ay = aidat.Ay;

            return View();
        }

        [HttpPost]
        public IActionResult OdemeTamamla(int aidatId, string kartAdSoyad, string kartNo)
        {
            var sakinAd = HttpContext.Session.GetString("SakinAd");
            if (string.IsNullOrEmpty(sakinAd)) return RedirectToAction("SakinGiris", "Home");

            var aidat = _context.Aidatlar.FirstOrDefault(a => a.Id == aidatId && a.SakinAdSoyad == sakinAd);
            if (aidat == null)
            {
                TempData["ErrorMessage"] = "Ödeme işlemi sırasında bir hata oluştu!";
                return RedirectToAction("Aidatlarim");
            }

            aidat.OdendiMi = true;
            aidat.KayitTarihi = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"{aidat.Ay} Dönemi Aidat Ödemesi Kredi Kartı ile Başarıyla Alındı.";
            return RedirectToAction("Aidatlarim");
        }
    }
}