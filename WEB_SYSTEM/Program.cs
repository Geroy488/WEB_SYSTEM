using Microsoft.EntityFrameworkCore;
using WEB_SYSTEM.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // adjust version as needed
    )
);


// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Configure the HTTP request pipeline.

app.UseRouting();           // <--- Add this before Authentication & Authorization

app.UseAuthentication();    // AuthN must come before AuthZ
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();