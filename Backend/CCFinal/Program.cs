using System.Text;
using CCFinal.Data;
using CCFinal.Mappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// DB Setup
builder.Services.AddDbContext<CCFinalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CCFinalContext") 
                         ?? throw new InvalidOperationException($"Connection string '{nameof(CCFinalContext)}' not found.")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));


// Identity Setup
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddHttpContextAccessor();

// Auth Setup
builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = false,
            //ValidateAudience = true,
            //ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "Development"))
        };
        options.RefreshOnIssuerKeyNotFound = true;
        options.AutomaticRefreshInterval = TimeSpan.FromHours(1);
    });


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Setup DI for task mapping
builder.Services.AddSingleton<ITodoMapper, TodoMapper>();

builder.Services.AddLogging();
builder.Logging.AddConsole();

builder.Services.AddCors(option => {
    option.DefaultPolicyName = "final";
    option.AddPolicy("final", policy => {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(host => true)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .Build();
    });
});

builder.Services.AddCap(options => {
    options.UseDashboard();
    options.UseEntityFramework<ApplicationDbContext>();
    options.UseKafka(builder.Configuration.GetSection("Kafka")["Servers"] ?? string.Empty);
});

var app = builder.Build();

app.UseCors("final");


// Force DB Migrations
using(var scope = app.Services.CreateScope())
{
    var ccFinalContext = scope.ServiceProvider.GetRequiredService<CCFinalContext>();

    try {
        await ccFinalContext.Database.EnsureCreatedAsync();
        //ccFinalContext.Seed();
    }
    catch (Exception ex) {
        app.Logger.LogInformation(ex, "Task database ensure creation");
    }

    var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try {
        await appContext.Database.EnsureCreatedAsync();
    }
    catch (Exception ex) {
        app.Logger.LogInformation(ex, "User database ensure creation");
    }
}

// OpenAPI browser for easier endpoint visualization
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
