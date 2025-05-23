using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class GameVibesAuthService {
    public static void AddAuthenticationGameVibesJwt(this IServiceCollection services,
        IConfiguration configuration) {
        services
            .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey
                        = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });
    }

    public static void AddAuthorizationGameVibes(this IServiceCollection services) {
        services
            .AddAuthorization(options => {
                options.AddPolicy("admin", policy => policy.RequireRole("admin"));
                options.AddPolicy("mod", policy => policy.RequireRole("mod"));
                options.AddPolicy("user", policy => policy.RequireRole("user"));
                options.AddPolicy("modOrAdmin", policy => policy.RequireRole("mod", "admin"));
            });
    }
}
