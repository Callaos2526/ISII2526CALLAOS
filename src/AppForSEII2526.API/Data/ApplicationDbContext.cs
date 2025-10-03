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



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BonosComprados>()
             .HasAlternateKey(bc => new { bc.CompraId, bc.BonoId });
    }
    
}
