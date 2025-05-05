using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ManualProg.Api.Core;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>("sqldb");
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

        _ = builder.Services.AddScoped<CurrentUserService>();

        builder.AddCors();

        builder.AddJwtAuthentication();
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
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
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
