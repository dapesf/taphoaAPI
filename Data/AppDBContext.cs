using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
public partial class AppDBContext : IdentityDbContext<ma_user>
{
    public AppDBContext() { }
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // remove "AspNet" regex in Identity database
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            string tableName = entityType.GetTableName().ToString();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
