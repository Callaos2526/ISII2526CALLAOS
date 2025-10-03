using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Producto> Producto { get; set; }
    public DbSet<TipoProducto> TipoProducto { get; set; }
    public DbSet<Compra_Producto> Compra_Producto { get; set; }
    public DbSet<Producto_Compra> Producto_Compra { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 
    }
}
