using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Bocadillo> Bocadillos { get; set; }
    public DbSet<TipoPan> TiposPan { get; set; }
    public DbSet<ResenyaBocadillo> ResenyasBocadillo { get; set; }
    public DbSet<Resenya> Resenyas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación uno a muchos: TipoPan -> Bocadillo
        modelBuilder.Entity<Bocadillo>()
            .HasOne(b => b.tipopan)
            .WithMany(tp => tp.Bocadillos)
            .HasForeignKey(b => b.TipoPanId);

        // Relación uno a muchos: Bocadillo -> ResenyaBocadillo
        modelBuilder.Entity<ResenyaBocadillo>()
            .HasOne(rb => rb.Bocadillo)
            .WithMany(b => b.ResenyaBocadillo)
            .HasForeignKey(rb => rb.BocadilloId);

        // Relación uno a muchos: Resenya -> ResenyaBocadillo
        modelBuilder.Entity<ResenyaBocadillo>()
            .HasOne(rb => rb.Resenya)
            .WithMany(r => r.ResenyaBocadillo)
            .HasForeignKey(rb => rb.ResenyaId);

        // Si quieres clave compuesta en ResenyaBocadillo:
        // modelBuilder.Entity<ResenyaBocadillo>()
        //     .HasKey(rb => new { rb.BocadilloId, rb.ResenyaId });
    }
}
