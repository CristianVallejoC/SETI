using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SETI.Areas.Identity.Data;
using SETI.Controllers;
using SETI.Models;

namespace SETI.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<DataGeneral> DataGeneral { get; set; }
    public DbSet<Broker> Broker { get; set; }
    public DbSet<DiscountRate> DiscountRate { get; set; }
    public DbSet<InvestmentProject> InvestmentProject { get; set; }
    public DbSet<InvestmentSector> InvestmentSector { get; set; }
    public DbSet<InvestmentSubsector> InvestmentSubsector { get; set; }
    public DbSet<Period> Period { get; set; }
    public DbSet<ProjectMovement> ProjectMovement { get; set; }
    public DbSet<Region> Region { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
