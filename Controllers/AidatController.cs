using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Data;
using ApartmanYonetimSistemi.Models;
using System.Linq;
using System;

namespace ApartmanYonetimSistemi.Controllers
{
    public class AidatController : Controller
    {
        private readonly AppDbContext _context;

        public AidatController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var aidatlar = _context.Aidatlar.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                aidatlar = aidatlar.Where(a => a.SakinAdSoyad.Contains(search) || a.Ay.Contains(search));
            }

            ViewBag.Sakinler = _context.Sakinler.ToList(); 

            return View(aidatlar.OrderByDescending(a => a.KayitTarihi).ToList());
        }

        [HttpPost]
        public IActionResult TopluAidatEkle(decimal tutar, string ay)
        {
            var sakinler = _context.Sakinler.ToList();

            foreach (var s in sakinler)
            {
                var yeniAidat = new Aidat
                {
                    Tutar = tutar,
                    Ay = ay,
                    SakinAdSoyad = s.AdSoyad,
                    OdendiMi = false,
                    KayitTarihi = DateTime.Now
                };
                _context.Aidatlar.Add(yeniAidat);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult TekliAidatEkle(decimal tutar, string ay, string sakinAd)
        {
            if (!string.IsNullOrEmpty(sakinAd))
            {
                var yeniAidat = new Aidat
                {
                    Tutar = tutar,
                    Ay = ay,
                    SakinAdSoyad = sakinAd,
                    OdendiMi = false,
                    KayitTarihi = DateTime.Now
                };

                _context.Aidatlar.Add(yeniAidat);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult OdendiYap(int id)
        {
            var aidat = _context.Aidatlar.Find(id);
            if (aidat != null)
            {
                aidat.OdendiMi = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            var a = _context.Aidatlar.Find(id);
            if (a != null)
            {
                _context.Aidatlar.Remove(a);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}