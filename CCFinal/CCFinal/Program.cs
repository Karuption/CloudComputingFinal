using CCFinal;using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CCFinal.Data;
using CCFinal.Mappers;
using Riok.Mapperly.Abstractions;
using TodoMapper = CCFinal.Mappers.TodoMapper;



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

//DI Setup
builder.Services.AddSingleton<ITodoMapper, TodoMapper>();

var app = builder.Build();

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
