using EventPlanner.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add controller get method to get users
app.MapGet("/users", async (IAppDbContext dbContext) =>
{
    return Results.Ok(await dbContext.Users.ToListAsync());
});

app.Run();