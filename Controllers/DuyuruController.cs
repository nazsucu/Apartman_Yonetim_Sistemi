using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Data;
using ApartmanYonetimSistemi.Models;
using System.Linq;
using System;

namespace ApartmanYonetimSistemi.Controllers
{
    public class DuyuruController : Controller
    {
        private readonly AppDbContext _context;

        public DuyuruController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var duyurular = _context.Duyurular.OrderByDescending(x => x.YayinTarihi).ToList();
            return View(duyurular);
        }

        [HttpPost]
        public IActionResult Ekle(Duyuru yeniDuyuru)
        {
            if (ModelState.IsValid)
            {
                _context.Duyurular.Add(yeniDuyuru);
                _context.SaveChanges();
                TempData["Mesaj"] = "Duyuru başarıyla yayınlandı!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            var duyuru = _context.Duyurular.Find(id);
            if (duyuru != null)
            {
                _context.Duyurular.Remove(duyuru);
                _context.SaveChanges();
                TempData["Mesaj"] = "Duyuru başarıyla silindi!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Guncelle(int id)
        {
            var duyuru = _context.Duyurular.Find(id);
            if (duyuru == null) return RedirectToAction("Index");
            return View(duyuru);
        }

        [HttpPost]
        public IActionResult Guncelle(Duyuru guncelDuyuru)
        {
            var eskiDuyuru = _context.Duyurular.Find(guncelDuyuru.Id);
            if (eskiDuyuru != null)
            {
                eskiDuyuru.Baslik = guncelDuyuru.Baslik;
                eskiDuyuru.Icerik = guncelDuyuru.Icerik;
                _context.SaveChanges();
                TempData["Mesaj"] = "Duyuru güncellendi!";
            }
            return RedirectToAction("Index");
        }
    }
}