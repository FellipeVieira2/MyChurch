using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyChurch.Api.Web.Configuration;
using MyChurch.Api.Web.Filters;
using MyChurch.Application;
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
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.InjectInfra(builder.Configuration);
builder.Services.InjectApplication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>());
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();