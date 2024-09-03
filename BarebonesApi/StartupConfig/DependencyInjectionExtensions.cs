using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace BarebonesApi.StartupConfig;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Add standard services to the container.

        builder.Services.AddControllers();

        // Add swagger services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Authorization services
        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        // Add health checks
        // SQL server required before activation, to append below - .AddSqlServer(builder.Configuration.GetConnectionString("Default"))
        builder.Services.AddHealthChecks();


        // Add authentication services
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey")))
            };
        });
    }
}
