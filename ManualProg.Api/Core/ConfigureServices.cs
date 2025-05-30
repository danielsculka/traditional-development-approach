﻿using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace ManualProg.Api.Core;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseLazyLoadingProxies()
                   .UseSqlServer(builder.Configuration.GetConnectionString("sqldb"));
        });

        builder.AddServiceDefaults();
        builder.AddSwagger();

        builder.Services.Configure<AuthOptions>(options =>
        {
            options.TokenSecurityKey = builder.Configuration.GetValue<string>("token-security-key")!;
        });

        _ = builder.Services.AddSingleton<IdentityService>();

        builder.Services.Configure<AdminOptions>(options =>
        {
            options.Username = builder.Configuration.GetValue<string>("admin-username")!;
            options.Password = builder.Configuration.GetValue<string>("admin-password")!;
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();

        builder.AddCors();

        builder.AddJwtAuthentication();

        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
            options.InferSecuritySchemes();
        });
    }

    private static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(o => o.AddPolicy("policy", builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }));
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("token-security-key")!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
    }
}
