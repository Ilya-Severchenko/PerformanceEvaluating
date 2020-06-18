
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

        public DbSet<RequestResult> RequestResults { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestResult>().ToTable("RequestResults");
            modelBuilder.Entity<RequestResult>().HasKey(_ => _.Id);
            modelBuilder.Entity<RequestResult>().Property(_ => _.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            base.OnModelCreating(modelBuilder);
        }
    }
}
