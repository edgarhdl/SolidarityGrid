namespace SolidarityGrid.Node.Domain;
public class Pago
{
    public Guid Id { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string QuienLoTiene { get; set; } = string.Empty;
}