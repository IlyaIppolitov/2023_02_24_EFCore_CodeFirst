﻿// <auto-generated />
using System;
using EFCore_CodeFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCore_CodeFirst.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230225072457_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.14");

            modelBuilder.Entity("EFCore_CodeFirst.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Birthday")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("EFCore_CodeFirst.Visitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Visitation");
                });

            modelBuilder.Entity("EFCore_CodeFirst.Visitation", b =>
                {
                    b.HasOne("EFCore_CodeFirst.Student", "Student")
                        .WithMany("Visitations")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("EFCore_CodeFirst.Student", b =>
                {
                    b.Navigation("Visitations");
                });
#pragma warning restore 612, 618
        }
    }
}