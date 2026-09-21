using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Attributes;

/// <summary>
/// Enforces system role authorization with automatic administrator bypass.
/// Inherits from AuthorizeAttribute to enforce authentication at the middleware stage (401 Unauthorized),
/// and implements IAsyncActionFilter to allow callers with the Admin claim to bypass role restrictions (403 Forbidden).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class AuthorizeRolesWithAdminBypassAttribute : AuthorizeAttribute, IAsyncActionFilter
{
    public UserRoleEnum[] AllowedRoles { get; }

    public AuthorizeRolesWithAdminBypassAttribute(params UserRoleEnum[] roles)
    {
        AllowedRoles = roles;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        // 1. Admin bypass: if user has Admin claim, allow immediately
        if (currentUserService.IsAdmin())
        {
            await next();
            return;
        }

        // 2. Role check: check if user holds one of the allowed roles
        var userRole = currentUserService.GetUserRole();
        if (userRole.HasValue && AllowedRoles.Contains(userRole.Value))
        {
            await next();
            return;
        }

        context.Result = new ForbidResult();
    }
}
