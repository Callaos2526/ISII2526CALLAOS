using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<TipoBocadillo> TiposBocadillos { get; set; }
    public DbSet<CompraBono> ComprasBono { get; set; }
    public DbSet<BonoBocadillo> BonosBocadillos { get; set; }
    public DbSet<BonosComprados> BonosComprados { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

       builder.Entity<BonosComprados>()//Unique key compuesto por CompraId y BonoId
            .HasAlternateKey(bc => new{bc.CompraId, bc.BonoId});
    }
    
}
