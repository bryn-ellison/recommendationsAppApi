﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecommendationsAppApiLibrary.DataAccess;
using System.Text;
using System.Text.Json.Serialization;


namespace RecommendationsApi.StartupConfig;

public static class DependencyInjectionExtensions
{
    public static void AddStandardServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddEndpointsApiExplorer();
        builder.AddSwaggerServices();
    }

    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISqliteDataAccess, SqliteDataAccess>();
        builder.Services.AddSingleton<IUserData, UserData>();
    }

    public static void AddCorsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
    }

    private static void AddSwaggerServices(this WebApplicationBuilder builder)
    {
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Description = "JWT Authorization header info using bearer tokens",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                new string[] {}
            }
        };

        builder.Services.AddSwaggerGen(opts =>
        {
            opts.AddSecurityDefinition("bearerAuth", securityScheme);
            opts.AddSecurityRequirement(securityRequirement);

            var title = "Recommendations App API";
            var description = "This is an API that supports the recommendations app being built by Bryn Ellison";
            var terms = new Uri("https://localhost:7000/terms");
            var contact = new OpenApiContact()
            {
                Name = "Bryn Ellison",
                Email = "help@brynellison.com",
                Url = new Uri("https://github.com/bryn-ellison/")
            };

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{title} v1",
                Description = description,
                TermsOfService = terms,
                Contact = contact
            });
        });
    }

    public static void AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey"))),
                ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
            };
        });
    }

    public static void AddAuthorizationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(opts =>
        {
            opts.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
    }

    public static void AddHealthCheckServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        // SQL server required before activation, to append above - .AddSqlServer(builder.Configuration.GetConnectionString("Default"))

        builder.Services.AddHealthChecksUI(opts =>
        {
            opts.AddHealthCheckEndpoint("api", "/health");
            opts.SetEvaluationTimeInSeconds(60);
            opts.SetMinimumSecondsBetweenFailureNotifications(10);
        }).AddInMemoryStorage();
    }


    public static void AddVersioningServices(this WebApplicationBuilder builder)
    {
        var apiVersioningBuilder = builder.Services.AddApiVersioning(opts =>
        {
            opts.ReportApiVersions = true;
            opts.DefaultApiVersion = new ApiVersion(1, 0);
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                            new HeaderApiVersionReader("x-api-version"),
                                            new MediaTypeApiVersionReader("x-api-version"));
        }); 

        apiVersioningBuilder.AddApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        }); 
    }
};