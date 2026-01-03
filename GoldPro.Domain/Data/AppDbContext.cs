using GoldPro.Application.Interfaces;
using GoldPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Domain.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantContext _tenant;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant)
            : base(options)
        {
            _tenant = tenant;
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<InvoiceSettings> InvoiceSettings => Set<InvoiceSettings>();
        public DbSet<NotificationSettings> NotificationSettings => Set<NotificationSettings>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Estimate> Estimates => Set<Estimate>();
        public DbSet<EstimateItem> EstimateItems => Set<EstimateItem>();
        public DbSet<OldGoldSlip> OldGoldSlips => Set<OldGoldSlip>();
        public DbSet<OldGoldItem> OldGoldItems => Set<OldGoldItem>();
        public DbSet<StockItem> StockItems => Set<StockItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(TenantEntity).IsAssignableFrom(entity.ClrType))
                {
                    // Build a lambda expression: (e) => e.Tenant.TenantId == _tenant.TenantId
                    var parameter = System.Linq.Expressions.Expression.Parameter(entity.ClrType, "e");
                    var tenantProperty = System.Linq.Expressions.Expression.Property(parameter, "Tenant");
                    var tenantIdProperty = System.Linq.Expressions.Expression.Property(tenantProperty, "TenantId");
                    var tenantIdValue = System.Linq.Expressions.Expression.Constant(_tenant.TenantId);
                    var equalExpression = System.Linq.Expressions.Expression.Equal(tenantIdProperty, tenantIdValue);
                    var lambda = System.Linq.Expressions.Expression.Lambda(equalExpression, parameter);

                    modelBuilder.Entity(entity.ClrType)
                        .HasQueryFilter(lambda);
                }
            }

            // Configure Sale relationship
            modelBuilder.Entity<Sale>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Items).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleItem>(b =>
            {
                b.HasKey(x => x.Id);
            });

            // Configure Order relationship
            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(b =>
            {
                b.HasKey(x => x.Id);
            });

            // Configure Estimate relationship
            modelBuilder.Entity<Estimate>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Items).WithOne(x => x.Estimate).HasForeignKey(x => x.EstimateId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EstimateItem>(b =>
            {
                b.HasKey(x => x.Id);
            });

            // Configure OldGoldSlip relationship
            modelBuilder.Entity<OldGoldSlip>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Items).WithOne(x => x.Slip).HasForeignKey(x => x.SlipId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OldGoldItem>(b =>
            {
                b.HasKey(x => x.Id);
            });

            // Configure StockItem
            modelBuilder.Entity<StockItem>(b =>
            {
                b.HasKey(x => x.Id);
            });
        }
    }

}
