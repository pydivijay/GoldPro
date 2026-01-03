using GoldPro.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using System;

namespace GoldPro.Shared.Tenant;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITenantContext tenant)
    {
        var user = context.User;

        // If request is not authenticated, skip tenant resolution (allows public endpoints like signup/login)
        if (user?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var tenantId = user.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new UnauthorizedAccessException("Tenant not found in token");

        if (tenant is not null)
        {
            // TenantContext implementation in Domain project has settable TenantId; cast and set if possible
            var tenantType = tenant.GetType();
            var prop = tenantType.GetProperty("TenantId");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(tenant, Guid.Parse(tenantId));
            }
        }

        await _next(context);
    }
}
