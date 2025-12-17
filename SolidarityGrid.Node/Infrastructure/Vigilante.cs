using Microsoft.EntityFrameworkCore;
using SolidarityGrid.Node.Domain;

namespace SolidarityGrid.Node.Infrastructure;

public class Vigilante : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly string _miNombre;

    public Vigilante(IServiceProvider sp)
    {
        _sp = sp;
        _miNombre = Environment.GetEnvironmentVariable("NODE_NAME") ?? "Anonimo";
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BaseDatos>();

                // 1. Aviso que estoy vivo
                var yo = await db.Estados.FindAsync(_miNombre);
                if (yo == null) db.Estados.Add(new EstadoNodo { Nombre = _miNombre, UltimaVez = DateTime.UtcNow });
                else yo.UltimaVez = DateTime.UtcNow;
                await db.SaveChangesAsync();

                // 2. Revisar por los otros nodos 
                var limite = DateTime.UtcNow.AddSeconds(-10); // tiempo de revision
                var caidos = await db.Estados
                    .Where(e => e.Nombre != _miNombre && e.UltimaVez < limite)
                    .ToListAsync();

                foreach (var nodoCaido in caidos)
                {
                    int filasRescatadas = await db.Pagos
                        .Where(p => p.QuienLoTiene == nodoCaido.Nombre && p.Estado == "Procesando")
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.QuienLoTiene, _miNombre)
                            .SetProperty(p => p.Estado, "Completado"), ct);

                    if (filasRescatadas > 0)
                    {
                        Console.WriteLine($"[{_miNombre}]: Detecté caída de {nodoCaido.Nombre}. Rescaté y completé {filasRescatadas} pago(s).");
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"[{_miNombre}] Error en ciclo de vigilancia: {ex.Message}");
            }

            await Task.Delay(3000, ct); // Repite cada 3 segundos
        }
    }
}