using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
public partial class TaphoaEntities : IdentityDbContext<ma_user>
{
    public TaphoaEntities() { }
    public TaphoaEntities(DbContextOptions<TaphoaEntities> options)
        : base(options)
    {
    }

    public DbSet<ma_literal> ma_literal { get; set; }
    public DbSet<ma_store> ma_store { get; set; }
    public DbSet<ma_user> ma_user { get; set; }
    public DbSet<tr_anounce> tr_anounce { get; set; }
    public DbSet<tr_product> tr_product { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
        IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("taphoa");
            optionsBuilder.UseNpgsql(connectionString);
     }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
