using Microsoft.AspNetCore.Mvc;
using ApartmanYonetimSistemi.Data;
using ApartmanYonetimSistemi.Models;
using System.Linq;

namespace ApartmanYonetimSistemi.Controllers
{
    public class HarcamaController : Controller
    {
        private readonly AppDbContext _context;

        public HarcamaController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var harcamalar = _context.Harcamalar.OrderByDescending(x => x.Tarih).ToList();

            decimal toplamGider = harcamalar.Sum(x => x.Tutar);

    
            decimal toplamGelir = _context.Aidatlar.Where(a => a.OdendiMi).Sum(a => a.Tutar);

           
            ViewBag.ToplamHarcama = toplamGider;
            ViewBag.KasaBakiye = toplamGelir - toplamGider;

            return View(harcamalar);
        }


        [HttpPost]
        public IActionResult Ekle(Harcama yeniHarcama)
        {
            if (ModelState.IsValid)
            {
                _context.Harcamalar.Add(yeniHarcama);
                _context.SaveChanges();
                TempData["Mesaj"] = "Harcama başarıyla kaydedildi!";
            }
            return RedirectToAction("Index");
        }

        
        public IActionResult Sil(int id)
        {
            var harcama = _context.Harcamalar.Find(id);
            if (harcama != null)
            {
                _context.Harcamalar.Remove(harcama);
                _context.SaveChanges();
                TempData["Mesaj"] = "Harcama kaydı silindi!";
            }
            return RedirectToAction("Index");
        }
    }
}