using EventPlanner.Data;
using EventPlanner.Repository;
using EventPlanner.Business;
using EventPlanner.Services;
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
        ConfigureBusinessServices(services);
        ConfigureControllers(services);

        // Add services to the container.
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers();
    }

    private void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventsRepository, EventRepository>();
        services.AddScoped<IUserAvailabilityRepository, UserAvailabilityRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
    }

    private void ConfigureBusinessServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEventService, EventService>();
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
            endpoints.MapControllers();
        });

    }
}