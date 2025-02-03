using Microsoft.EntityFrameworkCore;
using PANDA.Api.Models;

namespace PANDA.Api.Data
{
    public class PandaContext : DbContext
    {
        public PandaContext(DbContextOptions<PandaContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.PatientEntity)
                        .WithMany(p => p.Appointments)
                        .HasForeignKey(a => a.Patient);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Clinician)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.ClinicianId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Organisation)
                .WithMany()
                .HasForeignKey(a => a.OrganisationId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId);
        }
    }
}
