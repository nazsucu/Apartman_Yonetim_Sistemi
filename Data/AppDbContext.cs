using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Sakin> Sakinler { get; set; }
        public DbSet<Yonetici> Yoneticiler { get; set; }
        public DbSet<ArizaTalebi> Arizalar { get; set; }
        public DbSet<Yonetici> Ayarlar { get; set; }
        public DbSet<Aidat> Aidatlar { get; set; }
        public DbSet<Duyuru> Duyurular { get; set; }
        public DbSet<GenelTalep> GenelTalepler { get; set; }
        public DbSet<Harcama> Harcamalar { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Harcama>().Property(h => h.Tutar).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Aidat>().Property(a => a.Tutar).HasColumnType("decimal(18,2)");
        }
    }
}