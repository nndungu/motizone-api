using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllers();
        
        services.AddEndpointsApiExplorer(); 
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(10, 11, 6))
            ));

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(RolePolicy.Admin, policy => policy.RequireRole(RolePolicy.Admin));
            options.AddPolicy(RolePolicy.Customer, policy => policy.RequireRole(RolePolicy.Customer));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        services.AddTransient<SmtpEmailSender>();

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                { 
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MotiZone API v1");
                    c.RoutePrefix = "swagger"; 
                });
        }
        else 
        {
            app.UseExceptionHandler("/Error");
        }

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
