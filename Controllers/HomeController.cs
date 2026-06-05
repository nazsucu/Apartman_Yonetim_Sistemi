using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Models;
using System.Collections.Generic;
using System.Linq;
using ApartmanYonetimSistemi.Data;
using System;
using Microsoft.AspNetCore.Http;

namespace ApartmanYonetimSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Welcome() => View();
        public IActionResult Index() => View();
        public IActionResult SakinGiris() => View();

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("SakinId") != null)
            {
                return RedirectToAction("Dashboard", "SakinPanel");
            }

            ViewBag.ToplamDaire = _context.Sakinler.Count();
            ViewBag.BekleyenTalep = _context.GenelTalepler.Count();

            var toplamGelir = _context.Aidatlar.Where(x => x.OdendiMi).Sum(x => (decimal?)x.Tutar) ?? 0;
            var toplamGider = _context.Harcamalar.Sum(x => (decimal?)x.Tutar) ?? 0;
            var netKasa = toplamGelir - toplamGider;

            ViewBag.Kasa = netKasa.ToString("N0");

            var toplamAidatSayisi = _context.Aidatlar.Count();
            var odenenAidatSayisi = _context.Aidatlar.Count(x => x.OdendiMi);

            double yuzde = 0;
            if (toplamAidatSayisi > 0)
            {
                yuzde = (double)odenenAidatSayisi / toplamAidatSayisi * 100;
            }
            ViewBag.AidatYuzde = Math.Round(yuzde);

            var sonTalepler = _context.GenelTalepler
                                      .OrderByDescending(x => x.Tarih)
                                      .ToList();

            return View(sonTalepler);
        }

        [HttpPost]
        public IActionResult SakinKayit(Sakin yeniSakin, string AdSoyad, string adSoyad, string Email, string Telefon, string Sifre, string sifre, string password, string KullaniciAdi, string kullaniciAdi, int? KapiNo, int? daireNo, int? DaireNo)
        {
            string gelenAd = !string.IsNullOrEmpty(yeniSakin?.AdSoyad) ? yeniSakin.AdSoyad :
                             (!string.IsNullOrEmpty(AdSoyad) ? AdSoyad : (!string.IsNullOrEmpty(adSoyad) ? adSoyad : ""));

            string gelenSifre = "";
            if (yeniSakin != null && !string.IsNullOrEmpty(yeniSakin.Sifre)) gelenSifre = yeniSakin.Sifre;
            else if (!string.IsNullOrEmpty(Sifre)) gelenSifre = Sifre;
            else if (!string.IsNullOrEmpty(sifre)) gelenSifre = sifre;
            else if (!string.IsNullOrEmpty(password)) gelenSifre = password;

            if (string.IsNullOrEmpty(gelenAd.Trim()) || string.IsNullOrEmpty(gelenSifre.Trim()))
            {
                return Json(new { success = false, message = "Ad Soyad ve Şifre alanları boş bırakılamaz!" });
            }

            var sakin = new Sakin
            {
                AdSoyad = gelenAd.Trim(),
                Email = !string.IsNullOrEmpty(yeniSakin?.Email) ? yeniSakin.Email : (!string.IsNullOrEmpty(Email) ? Email.Trim() : ""),
                Telefon = !string.IsNullOrEmpty(yeniSakin?.Telefon) ? yeniSakin.Telefon : (!string.IsNullOrEmpty(Telefon) ? Telefon.Trim() : ""),
                Sifre = gelenSifre.Trim()
            };

            int bulunanDaireNo = 0;
            if (yeniSakin != null && yeniSakin.DaireNo > 0) bulunanDaireNo = yeniSakin.DaireNo;
            else if (DaireNo.HasValue && DaireNo.Value > 0) bulunanDaireNo = DaireNo.Value;
            else if (daireNo.HasValue && daireNo.Value > 0) bulunanDaireNo = daireNo.Value;
            else if (KapiNo.HasValue && KapiNo.Value > 0) bulunanDaireNo = KapiNo.Value;

            sakin.DaireNo = bulunanDaireNo > 0 ? bulunanDaireNo : 1;


            string gelenKullaniciAdi = "";
            if (yeniSakin != null && !string.IsNullOrEmpty(yeniSakin.KullaniciAdi)) gelenKullaniciAdi = yeniSakin.KullaniciAdi;
            else if (!string.IsNullOrEmpty(KullaniciAdi)) gelenKullaniciAdi = KullaniciAdi;
            else if (!string.IsNullOrEmpty(kullaniciAdi)) gelenKullaniciAdi = kullaniciAdi;

            if (!string.IsNullOrEmpty(gelenKullaniciAdi))
            {
                sakin.KullaniciAdi = gelenKullaniciAdi.Trim();
            }
            else
            {
                string temizAd = sakin.AdSoyad.ToLower()
                    .Replace(" ", "")
                    .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                    .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

                sakin.KullaniciAdi = temizAd + sakin.DaireNo;
            }

            _context.Sakinler.Add(sakin);
            _context.SaveChanges();

            return Json(new { success = true, message = "Sakin kaydı başarıyla oluşturuldu." });
        }

        public IActionResult CikisYap()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SakinLogin(string AdSoyad, string KullaniciAdi, string Sifre)
        {
            var sakin = _context.Sakinler.FirstOrDefault(s =>
                (s.KullaniciAdi == KullaniciAdi || s.AdSoyad == KullaniciAdi || s.Email == KullaniciAdi ||
                 s.KullaniciAdi == AdSoyad || s.AdSoyad == AdSoyad || s.Email == AdSoyad)
                && s.Sifre == Sifre);

            if (sakin != null)
            {
                HttpContext.Session.SetInt32("SakinId", sakin.Id);
                HttpContext.Session.SetString("SakinAd", sakin.AdSoyad);
                HttpContext.Session.SetString("SakinDaireNo", sakin.DaireNo.ToString());

                return Json(new { success = true, redirectUrl = "/SakinPanel/Dashboard", message = "Apartman Sakini Girişi Başarılı!" });
            }

            var admin = _context.Yoneticiler.FirstOrDefault(a => a.KullaniciAdi == KullaniciAdi && a.Sifre.Trim() == Sifre.Trim());
            if (admin != null)
            {
                HttpContext.Session.SetString("YoneticiAd", admin.AdSoyad);
                return Json(new { success = true, redirectUrl = "/Admin/Dashboard", message = "Yönetici Girişi Başarılı!" });
            }

            return Json(new { success = false, message = "Hatalı giriş!" });
        }
    }
}