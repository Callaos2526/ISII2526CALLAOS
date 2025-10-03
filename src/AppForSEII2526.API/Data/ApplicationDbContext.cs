using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<Bocadillo> Bocadillos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<CompraBocadillo> ComprasBocadillos { get; set; }
    public DbSet<TipoPan> TiposPan { get; set; }
    public DbSet<MetodoPago> MetodosPago { get; set; }
    public DbSet<GooglePay> GooglePay { get; set; }
    public DbSet<Tarjeta> Tarjeta { get; set; }
    public DbSet<Paypal> Paypal { get; set; }
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        //Composite key for CompraBocadillo
        builder.Entity<CompraBocadillo>()
            .HasKey(cb => new { cb.CompraId, cb.BocadilloId });
        
    }
}
