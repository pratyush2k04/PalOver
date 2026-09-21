using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params UserRoleEnum[] roles)
    {
        Roles = string.Join(",", roles.Select(r => r.ToRoleName()));
    }
}
