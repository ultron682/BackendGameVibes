using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackendGameVibes.Helpers {
    public class AuthorizeCheckOperationFilter : IOperationFilter {
        public void Apply(OpenApiOperation operation, OperationFilterContext context) {
            // Sprawdza, czy endpoint ma atrybut [Authorize]
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                .OfType<AuthorizeAttribute>().Any()
                                || context.MethodInfo.GetCustomAttributes(true)
                                .OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize) {
                // Dodaje wymaganie zabezpieczenia
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                }
            };
            }
        }
    }
}
