using Microsoft.OpenApi.Models;
using MyChurch.Api.Web.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using System.Reflection;
using Serilog;

namespace MyChurch.Api.Web.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API MyChurch",
                    Description = "Api To Manage MyChurch."
                });

                string[] methodsOrder = new string[5] { "post", "get", "put", "patch", "delete" };
                c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{Array.IndexOf(methodsOrder, apiDesc.HttpMethod.ToLower())}");
                c.OperationFilter<JsonIgnoreQueryOperationFilter>();
                c.OperationFilter<JsonIgnorePathOperationFilter>();
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                AddSwaggerXml(c);

                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                //    Name = "Authorization",
                //    Scheme = "Bearer",
                //    BearerFormat = "JWT",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        new string[] {}
                //    }
                //});
            });

            services.AddFluentValidationRulesToSwagger();
            return services;
        }
        public static void AddSwaggerXml(SwaggerGenOptions c)
        {
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var xmlFile in xmlFiles)
            {
                c.IncludeXmlComments(xmlFile);
            }
        }
        public static WebApplication UseSwaggerConfiguration(this WebApplication app, IWebHostEnvironment environment)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty; // Deixa o Swagger na raiz (http://localhost:<porta>/)
            });

            return app;
        }
    }
}
