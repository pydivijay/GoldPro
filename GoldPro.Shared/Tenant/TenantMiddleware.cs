using GoldPro.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
        var tenantId = context.User?.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new UnauthorizedAccessException("Tenant not found in token");

        if (tenant is not null && tenant is ITenantContext)
        {
            // TenantContext implementation in Domain project has settable TenantId; cast and set
            if (tenant is TenantContext cast)
            {
                cast.TenantId = Guid.Parse(tenantId);
            }
            else
            {
                // try to set via reflection as fallback
                var prop = tenant.GetType().GetProperty("TenantId");
                prop?.SetValue(tenant, Guid.Parse(tenantId));
            }
        }

        await _next(context);
    }
}
