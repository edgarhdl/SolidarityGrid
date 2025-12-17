using Microsoft.EntityFrameworkCore;
using SolidarityGrid.Node.Domain;
namespace SolidarityGrid.Node.Infrastructure;

public class BaseDatos : DbContext
{
    public BaseDatos(DbContextOptions<BaseDatos> options) : base(options) { }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<EstadoNodo> Estados { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<EstadoNodo>().HasKey(e => e.Nombre);
    }
}