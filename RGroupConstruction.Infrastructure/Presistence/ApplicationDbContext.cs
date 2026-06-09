using RGroupConstruction.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RGroupConstruction.Infrastructure.Presistence;

public class ApplicationDbContext(
DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, string>(options)
{
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Ads> Ads { get; set; }
    public required DbSet<AdsTranslation> AdsTranslations { get; set; }
    public required DbSet<Department> Department { get; set; }
    public required DbSet<Job> Jobs { get; set; }
    public required DbSet<JobTranslation> JobTranslations { get; set; }
    public required DbSet<JobApplication> JobApplications { get; set; }
    public required DbSet<CompanyInfo> CompanyInfos { get; set; }
    public required DbSet<CompanyInfoTranslation> CompanyInfoTranslations { get; set; }
    public required DbSet<Layout> Layouts { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }
    public required DbSet<Subscribe> Subscribes { get; set; }
    public required DbSet<UserNotification> UserNotifications { get; set; }
    public required DbSet<Status> Statuses { get; set; }
    public required DbSet<City> Cities { get; set; }
    public required DbSet<Project> Projects { get; set; }
    public required DbSet<ProjectTranslation> ProjectTranslations { get; set; }
    public required DbSet<ProjectImage> ProjectImages { get; set; }
    public required DbSet<Unit> Units { get; set; }
    public required DbSet<UnitImage> UnitImages { get; set; }
    public required DbSet<ContactMessage> ContactMessages { get; set; }
    public required DbSet<LogEntry> Logs { get; set; }
    public required DbSet<Parking> Parkings { get; set; }
    public required DbSet<UnitClient> UnitClients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LogEntry>(entity =>
        {
            entity.ToTable("Logs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.Id).HasColumnName("id");
            entity.Property(log => log.Timestamp).HasColumnName("Timestamp");
            entity.Property(log => log.Level).HasColumnName("Level");
            entity.Property(log => log.Template).HasColumnName("Template");
            entity.Property(log => log.Message).HasColumnName("Message");
            entity.Property(log => log.Exception).HasColumnName("Exception");
            entity.Property(log => log.Properties).HasColumnName("Properties");
            entity.Property(log => log.CreatedAt).HasColumnName("_ts");
        });
    }
}

