using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<TipoBocadillo> TiposBocadillos { get; set; }
    public DbSet<CompraBono> ComprasBono { get; set; }
    public DbSet<BonoBocadillo> BonosBocadillos { get; set; }
    public DbSet<BonosComprados> BonosComprados { get; set; }
    public DbSet<Tarjeta> Tarjetas { get; set; }
    public DbSet<GooglePay> GooglePays { get; set; }
    public DbSet<Paypal> Paypals { get; set; }
    public DbSet<Producto> Producto { get; set; }
    public DbSet<TipoProducto> TipoProducto { get; set; }
    public DbSet<Compra_Producto> Compra_Producto { get; set; }
    public DbSet<Producto_Compra> Producto_Compra { get; set; }
    public DbSet<Bocadillo> Bocadillos { get; set; }
    public DbSet<TipoPan> TiposPan { get; set; }
    public DbSet<ResenyaBocadillo> ResenyasBocadillo { get; set; }
    public DbSet<Resenya> Resenyas { get; set; }
    public DbSet<Bocadillo> Bocadillos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<CompraBocadillo> ComprasBocadillos { get; set; }
    public DbSet<TipoPan> TiposPan { get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BonosComprados>()
             .HasAlternateKey(bc => new { bc.CompraId, bc.BonoId });
        modelBuilder.Entity<ResenyaBocadillo>()
            .HasKey(rb => new { rb.BocadilloId, rb.ResenyaId });
        builder.Entity<CompraBocadillo>()
           .HasKey(cb => new { cb.CompraId, cb.BocadilloId });
    }
    

    
    

