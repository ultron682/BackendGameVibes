namespace BackendGameVibes;

using BackendGameVibes.BackgroundServices;
using BackendGameVibes.Data;
using BackendGameVibes.Helpers;
using BackendGameVibes.IServices;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.Middlewares;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Points;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services;
using BackendGameVibes.Services.Forum;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;


public class Program {
    private static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Services to the container
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(
                builder => {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            c.OperationFilter<AuthorizeCheckOperationFilter>();
            c.EnableAnnotations();
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("GameVibesDbConnection"),
                new MySqlServerVersion(new Version(8, 0, 26)),
                mySqlOptions => { mySqlOptions.EnableRetryOnFailure(); mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }));

        builder.Services
            .AddIdentity<UserGameVibes, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT authentication
        builder.Services.AddAuthenticationGameVibesJwt(builder.Configuration);

        builder.Services.AddAuthorizationGameVibes();

        builder.Services.AddMemoryCache();

        builder.Services.Configure<IdentityOptions>(options => {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
        builder.Services.Configure<ExperiencePointsSettings>(builder.Configuration.GetSection("ExperiencePointsSettings"));

        builder.Services.AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        builder.Services.AddHostedService<BackgroundServiceRefreshSteamData>();

        builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
        builder.Services.AddSingleton<HtmlTemplateService>();
        builder.Services.AddSingleton<ISteamService, SteamService>();

        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IForumRoleService, ForumRoleService>();
        builder.Services.AddScoped<IForumThreadService, ForumThreadService>();
        builder.Services.AddScoped<IForumPostService, ForumPostService>();
        builder.Services.AddScoped<IAdministrationService, AdministrationService>();
        builder.Services.AddScoped<IActionCodesService, ActionCodesService>();

        builder.Services.AddTransient<IForumExperienceService, ForumExperienceService>();
        builder.Services.AddTransient<MailService>();

        builder.Services.AddHttpClient();

        builder.Services.AddHealthChecks()
            .AddCheck<GameVibesHealthDbCheck>("game_vibes_db_health", tags: ["db"])
            .AddCheck<GameVibesHealthInternetCheck>("game_vibes_internet_health", tags: ["internet"]);


        var app = builder.Build();

        app.MapHealthChecks("api/health", new HealthCheckOptions {
            Predicate = _ => true,
            ResponseWriter = CustomHealthCheckResponseWriter.WriteCustomHealthCheckResponse
        });

        // Always available Swagger
        app.UseSwagger();
        app.UseSwaggerUI(o => {
            o.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            o.HeadContent = "<style> .topbar { display: none; } </style>";
            o.EnablePersistAuthorization();
            o.EnableTryItOutByDefault();
        });

        app.UseCors();
        app.UseRouting();

        app.UseAuthentication();

        app.UseMiddleware<UpdateLastActivityMiddleware>();

        app.UseAuthorization();

        app.MapControllers();



        ISteamService? steamService = app.Services.GetService<ISteamService>(); // on start backend download steam games IDs
        _ = steamService!.InitSteamApi();

        using (var scope = app.Services.CreateAsyncScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            //await dbContext.Database.MigrateAsync();
            await DbInitializer.InitializeAsync(scope, dbContext);
        }

        app.Run();
    }
}