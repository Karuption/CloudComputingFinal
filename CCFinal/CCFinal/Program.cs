using CCFinal.Data;
using CCFinal.Mappers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB Setup
builder.Services.AddDbContext<CCFinalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CCFinalContext") 
                         ?? throw new InvalidOperationException("Connection string 'CCFinalContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI Setup
builder.Services.AddSingleton<ITodoMapper, TodoMapper>();

builder.Services.AddCors(option => {
    option.AddPolicy("final", policy => {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
        policy.DisallowCredentials();
        policy.SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();

app.UseCors("final");

//DB Setup
using(var scope = app.Services.CreateScope())
{
    var ccFinalContext = scope.ServiceProvider.GetRequiredService<CCFinalContext>();

    try {
        ccFinalContext.Database.EnsureCreated();
        //ccFinalContext.Seed();
    }
    catch (Exception ex) {}
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();

app.Run();
