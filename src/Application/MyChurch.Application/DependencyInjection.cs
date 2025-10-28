using System.Reflection;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyChurch.Domain.Behauviours;
using MyChurch.Application.Engagement;
using MyChurch.Application.Presentation.Services;
using MyChurch.Application.Services;
using MyChurch.Application.CashFlow.Services; // 🔥 NOVO
using Mychurch.Common.Services;

namespace MyChurch.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddScoped<IEngagementService, EngagementService>();
            services.AddScoped<ContentGenerationService>();
            services.AddScoped<IBibleService, BibleService>();
            services.AddScoped<IHymnService, HymnService>();
            
            // 🔥 NOVO - Serviço de Automação de CashFlow
            services.AddScoped<ICashFlowAutomationService, CashFlowAutomationService>();

            // Note: IReportGeneratorService should be registered in Web (implementation lives in Infrastructure)
            // services.AddScoped<IReportGeneratorService, ReportGeneratorService>();

            return services;
        }
    }
}
