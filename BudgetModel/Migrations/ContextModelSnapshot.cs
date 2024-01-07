﻿// <auto-generated />
using System;
using BudgetModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BudgetModel.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BudgetFileUser", b =>
                {
                    b.Property<string>("BudgetFilesId")
                        .HasColumnType("text");

                    b.Property<string>("OwnersId")
                        .HasColumnType("text");

                    b.HasKey("BudgetFilesId", "OwnersId");

                    b.HasIndex("OwnersId");

                    b.ToTable("BudgetFileUser");
                });

            modelBuilder.Entity("BudgetModel.Models.BudgetFile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("BudgetModel.Models.Category", b =>
                {
                    b.Property<string>("BudgetFileId")
                        .HasColumnType("text");

                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("DefaultType")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.HasKey("BudgetFileId", "Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BudgetModel.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BudgetFileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Period")
                        .HasColumnType("integer");

                    b.Property<DateTime>("RecordedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("BudgetFileId", "CategoryId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("BudgetModel.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FailedLoginCount")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastFailedLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BudgetFileUser", b =>
                {
                    b.HasOne("BudgetModel.Models.BudgetFile", null)
                        .WithMany()
                        .HasForeignKey("BudgetFilesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BudgetModel.Models.User", null)
                        .WithMany()
                        .HasForeignKey("OwnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BudgetModel.Models.Category", b =>
                {
                    b.HasOne("BudgetModel.Models.BudgetFile", "BudgetFile")
                        .WithMany("Categories")
                        .HasForeignKey("BudgetFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BudgetFile");
                });

            modelBuilder.Entity("BudgetModel.Models.Transaction", b =>
                {
                    b.HasOne("BudgetModel.Models.User", "Author")
                        .WithMany("AuthoredTransactions")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BudgetModel.Models.BudgetFile", "BudgetFile")
                        .WithMany("Transactions")
                        .HasForeignKey("BudgetFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BudgetModel.Models.User", "Owner")
                        .WithMany("Transactions")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BudgetModel.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("BudgetFileId", "CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("BudgetFile");

                    b.Navigation("Category");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("BudgetModel.Models.BudgetFile", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("BudgetModel.Models.User", b =>
                {
                    b.Navigation("AuthoredTransactions");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
