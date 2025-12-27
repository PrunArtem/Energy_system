using Microsoft.EntityFrameworkCore;
using EnergySystemAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 41))
    )
);

builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<PythonService>();
builder.Services.AddScoped<DecisionService>();

builder.Services.AddSingleton<ISimulationClock>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

    // Дістанемо стартовий час у окремому scope
    using var scope = scopeFactory.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var setting = db.Settings.FirstOrDefault(s => s.KeyName == "SimulationStartTime")
        ?? throw new Exception("SimulationStartTime not set");

    var startTime = DateTime.Parse(setting.Value);

    return new SimulationClock(startTime, scopeFactory);
});


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISettingsService, SettingsService>();
/*builder.Services.AddScoped<PythonService>();
builder.Services.AddScoped<DecisionService>();

builder.Services.AddSingleton<ISimulationClock>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    using var scope = scopeFactory.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var pythonS = scope.ServiceProvider.GetRequiredService<PythonService>();
    var decisS = scope.ServiceProvider.GetRequiredService<DecisionService>();

    var setting = db.Settings.FirstOrDefault(s => s.KeyName == "SimulationStartTime");
    if (setting == null) throw new Exception("SimulationStartTime not set");

    var startTime = DateTime.Parse(setting.Value);
    return new SimulationClock(startTime, pythonS, decisS);
});*/



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
