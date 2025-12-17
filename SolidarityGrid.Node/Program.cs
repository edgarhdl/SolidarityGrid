using Microsoft.EntityFrameworkCore;
using SolidarityGrid.Node.Infrastructure; 


var builder = WebApplication.CreateBuilder(args);


var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Data Source=solidarity.db";

builder.Services.AddDbContext<BaseDatos>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddHostedService<Vigilante>();


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BaseDatos>();
    db.Database.EnsureCreated(); 
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SolidarityGrid API V1");
    c.RoutePrefix = string.Empty; 
});

app.MapControllers();

app.Run();