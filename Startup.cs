using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotiZone.Authorization;
using MotiZone.Data;
using MotiZone.Models;
using MotiZone.Services;

namespace MotiZone;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllers();

        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(10, 11, 6)) // MariaDB version
            ));

        // Identity Setup
        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication Setup
        var jwtKey = Configuration["Jwt:Key"];
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = Configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true
            };
        });

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(RolePolicy.Admin, policy => policy.RequireRole(RolePolicy.Admin));
            options.AddPolicy(RolePolicy.Customer, policy => policy.RequireRole(RolePolicy.Customer));
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        // Email (example with SMTP)
        services.AddTransient<SmtpEmailSender>();

        // App Services
        services.AddScoped<VehicleService>();
        services.AddScoped<PartService>();
        services.AddScoped<UserService>();
        services.AddScoped<CartService>();
        services.AddScoped<OrderService>();
        services.AddScoped<PdfInvoiceService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<RentalService>();
        services.AddScoped<ImageService>();
        services.AddScoped<FinancingService>();
        services.AddScoped<CustomizationService>();
        services.AddScoped<JwtHelper>();
        /*services.AddHttpClient<DarajaGateway>();
        services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];*/
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        else app.UseExceptionHandler("/Error");

        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}
