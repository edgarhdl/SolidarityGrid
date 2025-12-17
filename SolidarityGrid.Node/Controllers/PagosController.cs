using Microsoft.AspNetCore.Mvc;
using SolidarityGrid.Node.Infrastructure; 
using SolidarityGrid.Node.Domain;
using Microsoft.EntityFrameworkCore;

namespace SolidarityGrid.Node.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class PagosController : ControllerBase 
    {
        private readonly BaseDatos _db;
      
        public PagosController(BaseDatos db)
        {
            _db = db;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pagar([FromBody] decimal monto)
        {
            var miNombre = Environment.GetEnvironmentVariable("NODE_NAME") ?? "Desconocido";

            var nuevoPago = new Pago
            {
                Id = Guid.NewGuid(),
                Monto = monto,
                Estado = "Procesando",
                QuienLoTiene = miNombre
            };

            _db.Pagos.Add(nuevoPago);
            await _db.SaveChangesAsync();

            // Tiempo de procesamiento del pago
            await Task.Delay(15000);

            nuevoPago.Estado = "Completado";
            await _db.SaveChangesAsync();

            return Ok(new { mensaje = $"Pago {nuevoPago.Id} hecho por {miNombre}" });
        }

        [HttpGet]
        public async Task<IActionResult> ListarPagos()
        {
            var pagos = await _db.Pagos.ToListAsync();
            return Ok(pagos);
        }
    }
}