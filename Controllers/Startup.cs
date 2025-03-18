using EventPlanner.Data;
using EventPlanner.Repository;
using EventPlanner.Business;
using Microsoft.EntityFrameworkCore;
using TgMiniAppAuth;

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

        services.AddHttpContextAccessor();

        ConfigureRepositories(services);
        ConfigureBusinessServices(services);
        ConfigureControllers(services);

        services.AddTgMiniAppAuth(_configuration);

        AddCors(services);

        // Add services to the container.
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void AddCors(IServiceCollection services)
    {
        services.AddCors(options =>
      {
          options.AddPolicy("AllowAll", builder =>
          {
              builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
          });
      });
    }

    private void ConfigureControllers(IServiceCollection services)
    {

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
    }

    private void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventsRepository, EventRepository>();
        services.AddScoped<IUserAvailabilityRepository, UserAvailabilityRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IPollRepository, PollRepository>();
    }

    private void ConfigureBusinessServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IPollService, PollService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}