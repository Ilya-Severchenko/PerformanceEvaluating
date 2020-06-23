using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using PerformanceEvaluating.Data.Migrations;
using PerformanceEvaluating.Data.Models;


namespace PerformanceEvaluating.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public ApplicationDbContext() : base("Default")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public DbSet<DomainRequestResult> DomainRequestResults { get; set; }
        public DbSet<ChildRequestResult> ChildRequestResults { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DomainRequestResult>().ToTable("DomainRequestResults");
            modelBuilder.Entity<DomainRequestResult>().HasKey(_ => _.Id);
            modelBuilder.Entity<DomainRequestResult>().Property(_ => _.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            base.OnModelCreating(modelBuilder);
        }
    }
}
