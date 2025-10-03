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
using MyChurch.Api.Web.Configuration;
using MyChurch.Api.Web.Filters;
using MyChurch.Api.Web.Middleware;
using MyChurch.Application;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;
using MyChurch.Infrastructure.Utils.Postmark;
using MyChurch.Infrastructure.Utils.SES;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

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

    // *** CORREÇÃO 2: Habilitar leitura do token da query string para o SignalR ***
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // Se a requisição for para um hub e tiver o token, configure o contexto
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/ws/worship"))) // Verifique o caminho do seu Hub aqui
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
builder.Services.AddScoped<IEmailService, PostmarkEmailService>();

var awsConfig = builder.Configuration.GetSection("AWS");
var awsCredentials = new BasicAWSCredentials(
    awsConfig["AccessKey"],
    awsConfig["SecretKey"]);
var awsRegion = RegionEndpoint.GetBySystemName(awsConfig["Region"]);
builder.Services.AddSignalR(hubOptions => {
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(15); // Aumentado para dar mais margem
}); builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = awsCredentials,
    Region = awsRegion
});
builder.Services.InjectS3(builder.Configuration);
builder.Services.InjectApplication();
builder.Services.AddHttpClient<IAsaasWebClient, AsaasWebClient>();
builder.Services.AddAuthorization();
builder.Services.AddControllers(options => options.Filters.Add<JwtMemberFilter>());
builder.Services.AddCors(delegate (CorsOptions options)
{
    options.AddPolicy("_myAllowSpecificOrigins", delegate (CorsPolicyBuilder policy)
    {
        policy.WithOrigins(
            "https://www.mychurchlab.net", // produção
            "http://localhost:3000",       // desenvolvimento local
            "https://localhost:3000",    // se usar https localmente
            "https://localhost:7265/",     // se usar https localmente
            "http://localhost:7265/"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Essencial para SignalR com autenticação
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
    MemberSeedData.SeedMembers(dbContext); // <-- Adiciona o seed de membros
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
});

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

// *** CORREÇÃO 1: ORDEM CORRETA DOS MIDDLEWARES ***
app.UseCors("_myAllowSpecificOrigins");

// Ativar Rate Limiting
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Registrar o middleware JWT
app.UseMiddleware<JwtMiddleware>();

// Registrar o middleware de histórico de ações do usuário
app.UseMiddleware<UserActionHistoryMiddleware>();

app.MapControllers();
app.MapHub<WorshipServiceHub>("/ws/worship"); // Mapeamento de endpoints por último
app.MapHub<CampaignHub>("/campaignHub"); // Mapeia o CampaignHub para SignalR
app.MapHub<GroupHub>("/hubs/group"); // Novo Hub para grupos pequenos

app.Run();