﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PANDA.Api.Data;

#nullable disable

namespace PANDA.Api.Migrations
{
    [DbContext(typeof(PandaContext))]
    [Migration("20250203134738_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("PANDA.Api.Models.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClinicianId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ClinicianId1")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DepartmentId1")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DepartmentId2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAttended")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OrganisationId1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Patient")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClinicianId");

                    b.HasIndex("ClinicianId1");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DepartmentId1");

                    b.HasIndex("DepartmentId2");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("OrganisationId1");

                    b.HasIndex("Patient");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("PANDA.Api.Models.Clinician", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clinicians");
                });

            modelBuilder.Entity("PANDA.Api.Models.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("PANDA.Api.Models.Organisation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrgCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("PANDA.Api.Models.Patient", b =>
                {
                    b.Property<string>("NhsNumber")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("NhsNumber");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("PANDA.Api.Models.Appointment", b =>
                {
                    b.HasOne("PANDA.Api.Models.Clinician", "Clinician")
                        .WithMany("Appointments")
                        .HasForeignKey("ClinicianId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PANDA.Api.Models.Clinician", null)
                        .WithMany("MissedAppointments")
                        .HasForeignKey("ClinicianId1");

                    b.HasOne("PANDA.Api.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PANDA.Api.Models.Department", null)
                        .WithMany("Appointments")
                        .HasForeignKey("DepartmentId1");

                    b.HasOne("PANDA.Api.Models.Department", null)
                        .WithMany("MissedAppointments")
                        .HasForeignKey("DepartmentId2");

                    b.HasOne("PANDA.Api.Models.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PANDA.Api.Models.Organisation", null)
                        .WithMany("Appointments")
                        .HasForeignKey("OrganisationId1");

                    b.HasOne("PANDA.Api.Models.Patient", "PatientEntity")
                        .WithMany("Appointments")
                        .HasForeignKey("Patient")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clinician");

                    b.Navigation("Department");

                    b.Navigation("Organisation");

                    b.Navigation("PatientEntity");
                });

            modelBuilder.Entity("PANDA.Api.Models.Clinician", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("MissedAppointments");
                });

            modelBuilder.Entity("PANDA.Api.Models.Department", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("MissedAppointments");
                });

            modelBuilder.Entity("PANDA.Api.Models.Organisation", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("PANDA.Api.Models.Patient", b =>
                {
                    b.Navigation("Appointments");
                });
#pragma warning restore 612, 618
        }
    }
}
