using GoldPro.Domain.Data;
using GoldPro.Application.Interfaces;
using GoldPro.Application.Services;
using GoldPro.Shared.Tenant;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Security.Claims;
using System.Linq;
using System;

var builder = WebApplication.CreateBuilder(args);

//  Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
            "https://gold-manage-pro-vijay.netlify.app/",
                "http://localhost:8080"                       // optional, if you test locally
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // Only use AllowCredentials if your frontend actually sends cookies/auth headers
        //.AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Tenant context and middleware
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

// Configure DbContext using Npgsql (Postgres)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                       ?? "Host=localhost;Database=GoldProDB;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Register application services
builder.Services.AddScoped<IBusinessProfileService, BusinessProfileService>();
builder.Services.AddScoped<IInvoiceSettingsService, InvoiceSettingsService>();
builder.Services.AddScoped<INotificationSettingsService, NotificationSettingsService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEstimateService, EstimateService>();
builder.Services.AddScoped<IOldGoldService, OldGoldService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Auth service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // For local testing you may disable HTTPS requirement; enable in production
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        var jwtKey = builder.Configuration["Jwt:Key"] ?? "ReplaceWithActualKey";
        var issuerConfig = builder.Configuration["Jwt:Issuer"] ?? string.Empty;
        var audienceConfig = builder.Configuration["Jwt:Audience"] ?? string.Empty;

        // Build lists that include configured values and common variants (e.g. without dots)
        var validIssuers = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrEmpty(issuerConfig)) validIssuers.Add(issuerConfig);
        var issuerVariant = issuerConfig.Replace(".", "");
        if (!string.IsNullOrEmpty(issuerVariant) && issuerVariant != issuerConfig) validIssuers.Add(issuerVariant);

        var validAudiences = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrEmpty(audienceConfig)) validAudiences.Add(audienceConfig);
        var audienceVariant = audienceConfig.Replace(".", "");
        if (!string.IsNullOrEmpty(audienceVariant) && audienceVariant != audienceConfig) validAudiences.Add(audienceVariant);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = "role",

            ValidIssuers = validIssuers,
            ValidAudiences = validAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var loggerFactory = ctx.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                var logger = loggerFactory?.CreateLogger("JwtBearer");
                var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
                logger?.LogDebug("OnMessageReceived - Authorization header present: {HasAuth}", !string.IsNullOrEmpty(authHeader));
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var loggerFactory = ctx.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                var logger = loggerFactory?.CreateLogger("JwtBearer");
                var sub = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                logger?.LogInformation("OnTokenValidated - token valid for subject: {Sub}", sub);
                var claims = ctx.Principal?.Claims.Select(c => new { c.Type, c.Value }).ToList();
                logger?.LogDebug("OnTokenValidated - claims: {Claims}", claims);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                var loggerFactory = ctx.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                var logger = loggerFactory?.CreateLogger("JwtBearer");
                logger?.LogWarning(ctx.Exception, "OnAuthenticationFailed - token validation failed");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

// Use Tenant middleware (must be after authentication so user is available)
app.UseMiddleware<TenantMiddleware>();

app.MapControllers();

app.Run();
