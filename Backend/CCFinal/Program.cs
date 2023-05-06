using System.Text;
using CCFinal;
using CCFinal.Data;
using CCFinal.Mappers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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
        options.Events = new JwtBearerEvents {
            OnMessageReceived = ctx => {
                if (string.IsNullOrWhiteSpace(ctx.Token))
                    ctx.Token = ctx.Request.Query["access_token"];
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = false,
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
        policy
            //.AllowAnyOrigin()
            .WithOrigins("http://*.localhost:80", "http://*.localhost:81", "http://*.localhost:5173",
                "http://*.papederson.tech:443")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(host => true)
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(90));
    });
});

builder.Services.AddCap(options => {
    options.UseDashboard();
    options.UseEntityFramework<ApplicationDbContext>();
    options.UseKafka(builder.Configuration.GetSection("Kafka")["Servers"] ?? string.Empty);
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddDbContextCheck<CCFinalContext>();

builder.Services.AddSignalR(opt => { opt.EnableDetailedErrors = true; });

var app = builder.Build();

app.UseCors("final");

app.Use((context, next) => {
    if (string.IsNullOrWhiteSpace(context.Response.Headers.AccessControlAllowOrigin)) {
        context.Response.Headers.AccessControlAllowOrigin = context.Request.Protocol + context.Request.Host;
        context.Response.Headers.AccessControlAllowMethods = "*";
        context.Response.Headers.AccessControlAllowHeaders = "*";
    }

    return next.Invoke();
});

app.UseHealthChecks("/healthcheck",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });


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

app.MapHub<UserNotification>("/UserNotification", opt => { opt.CloseOnAuthenticationExpiration = true; });

app.MapControllers();

app.Run();