using System.Text;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
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
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                errors = validationEx.Errors
            });
            await context.Response.WriteAsync(result);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                errors = "Internal Server Error."
            });
            await context.Response.WriteAsync(result);
        }
    });
});

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("_myAllowSpecificOrigins");

// Registrar o middleware JWT
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();
app.Run();