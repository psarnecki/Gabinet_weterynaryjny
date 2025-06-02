using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Models;

namespace VetClinicManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalMedication> AnimalMedications { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitUpdate> VisitUpdates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.HealthRecord)
                .WithOne(hr => hr.Animal)
                .HasForeignKey<HealthRecord>(hr => hr.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Animal>()
                .HasMany(a => a.Visits)
                .WithOne(v => v.Animal)
                .HasForeignKey(v => v.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Animals)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedVisits)
                .WithOne(v => v.AssignedVet)
                .HasForeignKey(v => v.AssignedVetId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Visit>()
                .HasMany(v => v.Updates)
                .WithOne(vu => vu.Visit)
                .HasForeignKey(vu => vu.VisitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VisitUpdate>()
                .HasMany<AnimalMedication>()
                .WithOne(am => am.VisitUpdate)
                .HasForeignKey(am => am.VisitUpdateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AnimalMedication>()
                .HasOne(am => am.Animal)
                .WithMany()
                .HasForeignKey(am => am.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnimalMedication>()
                .HasOne(am => am.Medication)
                .WithMany(m => m.AnimalMedications)
                .HasForeignKey(am => am.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
