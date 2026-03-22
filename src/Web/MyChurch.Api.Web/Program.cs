using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using DotnetGeminiSDK;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Api.Web.Filters;
using MyChurch.Application;
using MyChurch.Application.Church.Commands.CreateChurchWithAdminMember;
using MyChurch.Application.Departments.Services;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Services;
using MyChurch.Infrastructure;
using MyChurch.Infrastructure.BackgroundJobs;
using MyChurch.Infrastructure.Repositories;
using MyChurch.Infrastructure.Services;
using MyChurch.Infrastructure.Utils.Postmark;
using Serilog;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Path = System.IO.Path;
using MyChurch.Api.Web.Middleware;
using Mychurch.Common.Services;
using MyChurch.Infrastructure.Services.Reports;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                // Se é token de PlatformUser, adiciona NameIdentifier baseado em platform_user_id
                var platformUserId = identity.FindFirst("platform_user_id")?.Value;
                if (!string.IsNullOrWhiteSpace(platformUserId) && identity.FindFirst(ClaimTypes.NameIdentifier) == null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, platformUserId));
                }
            }
            return Task.CompletedTask;
        },
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
builder.Services.AddControllers(options => options.Filters.Add<JwtMemberFilter>())
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problem = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Type = "https://httpstatuses.com/400"
            };

            return new BadRequestObjectResult(problem);
        };
    });

// ? CORS configurado corretamente para Produção e Desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        if (builder.Environment.IsProduction())
        {
            // ?? PRODUÇÃO: Apenas domínio oficial
            policy.WithOrigins("https://www.mychurchlab.net", "http://localhost:3000", "http://localhost:5210/", "https://demoapp.top1soft.com.br/")
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
builder.Services.AddScoped<MyChurch.Domain.Contracts.IEmailService, SendGridEmailService>();
builder.Services.AddScoped<IReviewVerificationService, ReviewVerificationService>();
builder.Services.AddScoped<IDepartmentAccessService, DepartmentAccessService>();
builder.Services.AddScoped<IPlanAccessService, PlanAccessService>();
builder.Services.AddScoped<IPlanLimitService, PlanLimitService>();
// Register report generator (interface in Mychurch.Common, implementation in Infrastructure)
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddScoped<MyChurch.Infrastructure.Utils.SES.IEmailService, SendGridEmailService>();

// Background workers
builder.Services.AddHostedService<PendingTransfersWorker>();

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
    PlatformUserSeedData.SeedPlatformUsers(dbContext);
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

        if (exception is null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }

        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
        logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.ContentType = "application/problem+json";

        // Domínio: validação (FluentValidation + ValidationBehaviour + ThrowException)
        if (exception is MyChurch.Domain.Exceptions.ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problem = new ValidationProblemDetails(validationEx.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = validationEx.Message,
                Type = "https://httpstatuses.com/400"
            };

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        // Auth
        if (exception is UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://httpstatuses.com/401"
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var genericProblem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://httpstatuses.com/500",
            Detail = app.Environment.IsDevelopment() ? exception.Message : "An unexpected error occurred."
        };

        if (app.Environment.IsDevelopment())
        {
            genericProblem.Extensions["stackTrace"] = exception.StackTrace;
        }

        await context.Response.WriteAsJsonAsync(genericProblem);
    });
});

// Ativar Rate Limiting
app.UseRateLimiter();

// Auditoria de ações (salva histórico de requests autenticados)
app.UseMiddleware<UserActionHistoryMiddleware>();

// ? ADICIONAR JWT MIDDLEWARE (ANTES DE AUTHENTICATION)
app.UseMiddleware<MyChurch.Api.Web.Middleware.JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<WorshipServiceHub>("/ws/worship");
app.MapHub<CampaignHub>("/campaignHub");
app.MapHub<GroupHub>("/hubs/group");

app.Run();