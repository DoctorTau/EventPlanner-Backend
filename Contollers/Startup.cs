using EventPlanner.Data;
using EventPlanner.Repository;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                               Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        ConfigureRepositories(services);

        // Add services to the container.
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventsRepository, EventRepository>();
        services.AddScoped<IUserAvailabilityRepository, UserAvailabilityRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            // Add controller get method to get users
            endpoints.MapGet("/users", async (IAppDbContext dbContext) =>
            {
                return Results.Ok(await dbContext.Users.ToListAsync());
            });
        });
    }
}