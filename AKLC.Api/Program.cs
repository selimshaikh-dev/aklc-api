using AKLC.Application;
using AKLC.Infrastructure;
using AKLC.Infrastructure.Identity;
using AKLC.Infrastructure.Persistence.Seed;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;


var builder =
    WebApplication.CreateBuilder(args);


// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();


// =========================================================
// APPLICATION LAYER
// =========================================================

builder.Services.AddApplication();


// =========================================================
// INFRASTRUCTURE LAYER
// =========================================================

builder.Services.AddInfrastructure(
    builder.Configuration);


// =========================================================
// JWT SETTINGS
// =========================================================

var jwtSettings =
    builder.Configuration
        .GetSection(
            JwtSettings.SectionName)
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JWT configuration was not found.");


if (
    string.IsNullOrWhiteSpace(
        jwtSettings.Key)
)
{
    throw new InvalidOperationException(
        "JWT Key is missing.");
}


if (
    string.IsNullOrWhiteSpace(
        jwtSettings.Issuer)
)
{
    throw new InvalidOperationException(
        "JWT Issuer is missing.");
}


if (
    string.IsNullOrWhiteSpace(
        jwtSettings.Audience)
)
{
    throw new InvalidOperationException(
        "JWT Audience is missing.");
}


// =========================================================
// JWT AUTHENTICATION
// =========================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken =
            true;


        // Development:
        // false allows local HTTPS/dev certificate usage.
        //
        // Production:
        // true ensures HTTPS metadata requirement.
        options.RequireHttpsMetadata =
            !builder.Environment.IsDevelopment();


        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer =
                    true,

                ValidateAudience =
                    true,

                ValidateLifetime =
                    true,

                ValidateIssuerSigningKey =
                    true,

                ValidIssuer =
                    jwtSettings.Issuer,

                ValidAudience =
                    jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                ClockSkew =
                    TimeSpan.Zero
            };
    });


// =========================================================
// AUTHORIZATION
// =========================================================

builder.Services.AddAuthorization();


// =========================================================
// CORS
// =========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularClient",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// =========================================================
// SWAGGER
// =========================================================

builder.Services
    .AddEndpointsApiExplorer();


builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title =
                    "AKLC API",

                Version =
                    "v1",

                Description =
                    "Ammar Korean Language Center Web API"
            });


        // -----------------------------------------
        // JWT BEARER AUTHENTICATION
        // -----------------------------------------

        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Type =
                    SecuritySchemeType.Http,

                Scheme =
                    "bearer",

                BearerFormat =
                    "JWT",

                Description =
                    "Enter your JWT access token."
            });


        // -----------------------------------------
        // SECURITY REQUIREMENT
        // -----------------------------------------

        options.AddSecurityRequirement(
            document =>
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)
                    ] = []
                });
    });


// =========================================================
// BUILD APPLICATION
// =========================================================

var app =
    builder.Build();


// =========================================================
// DATABASE SEED
// =========================================================

using (
    var scope =
        app.Services.CreateScope()
)
{
    var adminSeeder =
        scope.ServiceProvider
            .GetRequiredService<
                AdminSeeder>();


    await adminSeeder
        .SeedAsync();
}


// =========================================================
// DEVELOPMENT / SWAGGER
// =========================================================

if (
    app.Environment
        .IsDevelopment()
)
{
    app.UseSwagger();


    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "AKLC API v1");


        options.RoutePrefix =
            "swagger";


        options.DocumentTitle =
            "AKLC API";
    });
}


// =========================================================
// HTTPS
// =========================================================

app.UseHttpsRedirection();


// =========================================================
// STATIC FILES
// =========================================================
// Required for:
// - Student photos
// - Payment proofs
// - Future Income/Expense attachments
// =========================================================

app.UseStaticFiles();


// =========================================================
// CORS
// =========================================================

app.UseCors(
    "AngularClient");


// =========================================================
// AUTHENTICATION
// =========================================================
// Must come before Authorization.
// =========================================================

app.UseAuthentication();


// =========================================================
// AUTHORIZATION
// =========================================================

app.UseAuthorization();


// =========================================================
// CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// RUN
// =========================================================

app.Run();