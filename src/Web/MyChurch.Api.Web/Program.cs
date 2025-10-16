using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using DotnetGeminiSDK;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Api.Web.Filters;
using MyChurch.Application;
using MyChurch.Application.Church.Commands.CreateChurchWithAdminMember;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Services;
using MyChurch.Infrastructure;
using MyChurch.Infrastructure.Repositories;
using MyChurch.Infrastructure.Services;
using MyChurch.Infrastructure.Utils.Postmark;
using MyChurch.Infrastructure.Utils.SES;
using Serilog;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Path = System.IO.Path;
using MyChurch.Api.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // Rate limiter para votos (previne spam)
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "vote-limiter", options =>
    {
        options.PermitLimit = 10; // 10 votos
        options.Window = TimeSpan.FromMinutes(1); // por minuto
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    // Rate limiter para criação de reviews
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "review-limiter", options =>
    {
        options.PermitLimit = 3; // 3 reviews
        options.Window = TimeSpan.FromMinutes(5); // a cada 5 minutos
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });

    // Rate limiter geral para API
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "api-limiter", options =>
    {
        options.PermitLimit = 100; // 100 requests
        options.Window = TimeSpan.FromMinutes(1); // por minuto
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    });

    // Rate limiter para uploads (geocoding, uploads de foto)
    rateLimiterOptions.AddSlidingWindowLimiter(policyName: "upload-limiter", options =>
    {
        options.PermitLimit = 20;
        options.Window = TimeSpan.FromMinutes(1);
        options.SegmentsPerWindow = 4;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });

    // Resposta padrão quando limite é atingido
    rateLimiterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests. Please try again later.",
                retryAfter = retryAfter.TotalSeconds
            }, cancellationToken: token);
        }
        else
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests. Please try again later."
            }, cancellationToken: token);
        }
    };
});

// Configurar autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/ws/worship")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.InjectInfra(builder.Configuration);
builder.Services.AddScoped<IAsaasWebClient, AsaasWebClient>();

// ?? Registrar serviço de validação de documentos
builder.Services.AddScoped<IDocumentValidator, DocumentValidator>();

var awsConfig = builder.Configuration.GetSection("AWS");
var awsCredentials = new BasicAWSCredentials(
    awsConfig["AccessKey"],
    awsConfig["SecretKey"]);
var awsRegion = RegionEndpoint.GetBySystemName(awsConfig["Region"]);
builder.Services.AddSignalR(hubOptions => {
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(15);
}); 
builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = awsCredentials,
    Region = awsRegion
});
builder.Services.InjectS3(builder.Configuration);
builder.Services.InjectApplication();
builder.Services.AddHttpClient<IAsaasWebClient, AsaasWebClient>();
builder.Services.AddControllers(options => options.Filters.Add<JwtMemberFilter>());

// ? CORS configurado corretamente para Produção e Desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        if (builder.Environment.IsProduction())
        {
            // ?? PRODUÇÃO: Apenas domínio oficial
            policy.WithOrigins("https://www.mychurchlab.net", "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            // ?? DESENVOLVIMENTO: Permite qualquer origem
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});

builder.Services.AddGeminiClient(config =>
{
    config.ApiKey = builder.Configuration["Gemini:ApiKey"];
    config.TextBaseUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash";
});

// Configurar Swagger para suportar JWT
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
});
builder.Services.AddFluentValidationRulesToSwagger();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateChurchWithAdminMemberCommand).Assembly));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IEmailService, PostmarkEmailService>(); // Serviço de email
builder.Services.AddScoped<IReviewVerificationService, ReviewVerificationService>();

// ?? Background Jobs (comentado até adicionar pacote Microsoft.Extensions.Hosting.Abstractions)
// builder.Services.AddHostedService<ExpiredPromotionsCleanupJob>();

var app = builder.Build();

// Seed default Bible reading plans
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<MyChurchDbContext>();
    
    // Apply migrations
    dbContext.Database.Migrate();
    
    // Seed data
    BibleReadingPlanSeedData.SeedDefaultPlans(dbContext);
    MemberSeedData.SeedMembers(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("_myAllowSpecificOrigins");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.ContentType = "application/json";

        if (exception is MyChurch.Domain.Exceptions.ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var result = System.Text.Json.JsonSerializer.Serialize(new { errors = validationEx.Errors });
            await context.Response.WriteAsync(result);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var result = System.Text.Json.JsonSerializer.Serialize(new { errors = "Internal Server Error." });
            await context.Response.WriteAsync(result);
        }
    });
});

// Ativar Rate Limiting
app.UseRateLimiter();

// ?? ADICIONAR JWT MIDDLEWARE (ANTES DE AUTHENTICATION)
app.UseMiddleware<MyChurch.Api.Web.Middleware.JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<WorshipServiceHub>("/ws/worship");
app.MapHub<CampaignHub>("/campaignHub");
app.MapHub<GroupHub>("/hubs/group");

app.Run();