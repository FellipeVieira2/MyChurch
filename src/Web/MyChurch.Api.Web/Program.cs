using System.Text;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Api.Web.Configuration;
using MyChurch.Api.Web.Filters;
using MyChurch.Api.Web.Middleware;
using MyChurch.Application;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

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
});

builder.Services.AddAuthorization();
builder.Services.InjectInfra(builder.Configuration);
builder.Services.AddScoped<IAsaasWebClient, AsaasWebClient>();

var awsConfig = builder.Configuration.GetSection("AWS");
var awsCredentials = new BasicAWSCredentials(
    awsConfig["AccessKey"],
    awsConfig["SecretKey"]);
var awsRegion = RegionEndpoint.GetBySystemName(awsConfig["Region"]);

builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = awsCredentials,
    Region = awsRegion
});
builder.Services.InjectS3(builder.Configuration);
builder.Services.InjectApplication();
builder.Services.AddHttpClient<IAsaasWebClient, AsaasWebClient>();
builder.Services.AddAuthorization();
builder.Services.AddControllers(options => options.Filters.Add<JwtMemberFilter>());
builder.Services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>());
builder.Services.AddCors(delegate (CorsOptions options)
{
    options.AddPolicy("_myAllowSpecificOrigins", delegate (CorsPolicyBuilder policy)
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
});

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("_myAllowSpecificOrigins");
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (FluentValidation.ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            errors
        });

        await context.Response.WriteAsync(result);
    }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            errors = ex.Errors
        });

        await context.Response.WriteAsync(result);
    }
});
// Registrar o middleware JWT
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();
app.Run();