using System;

namespace Pdr.Api.Attributes;

/// <summary>
/// Specifies that System Administrators are permitted to access the annotated action or controller,
/// bypassing engagement-level assignment or role restrictions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class AllowAdminAttribute : Attribute
{
}
