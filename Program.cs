using CarRentalAPI.Configuration;
using CarRentalAPI.Middleware;
using CarRentalAPI.Services;
using CarRentalAPI.Storage;

var builder = WebApplication.CreateBuilder(args);

// ─── Konfiguracja (appsettings.json → RentalSettings) ────────────────────────
builder.Services.Configure<RentalSettings>(
    builder.Configuration.GetSection(RentalSettings.SectionName));

// ─── Swagger ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Car Rental API",
        Version     = "v1",
        Description =
            "System wypożyczalni samochodów – Etap II\n\n" +
            "**Architektura warstwowa**: Controllers → Services → Storage\n\n" +
            "**Global Error Handling**: middleware przechwytuje DomainException, " +
            "NotFoundException, ValidationException i zwraca ujednolicony JSON.\n\n" +
            "**Soft-delete**: DELETE ustawia IsDeleted=true, rekord pozostaje w bazie.\n\n" +
            "**Konfiguracja reguł biznesowych** pochodzi z appsettings.json → RentalSettings."
    });
});

// ─── Dependency Injection ─────────────────────────────────────────────────────
builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddScoped<ICarService,    CarService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IRentalService, RentalService>();

var app = builder.Build();

// ─── Middleware ───────────────────────────────────────────────────────────────
// GlobalExceptionMiddleware MUSI być pierwszy – przechwytuje błędy z całego pipeline'u
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Rental API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
