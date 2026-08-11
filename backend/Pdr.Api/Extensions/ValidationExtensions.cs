using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Validators;

namespace Pdr.Api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();
        return services;
    }
}
