using MotiZone;
using MotiZone.Data;

var builder = WebApplication.CreateBuilder(args);

// Manually call Startup.cs
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

// ✅ Seed the database with initial users, roles, and sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();

    if (env.IsDevelopment())
    {
        await SeedData.InitializeAsync(services);   
    }
}

app.Run();