﻿// <auto-generated />
using System;
using DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Tech_Store_API.__DB.__Migrations
{
    [DbContext(typeof(TechStoreDB))]
    [Migration("20220911122548_Add_Identity_Entities")]
    partial class Add_Identity_Entities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Entities.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime?>("ActivatedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("activated_at");

                    b.Property<string>("ActivatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("activated_by");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("SYSDATETIME()");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasDefaultValue("app_dev")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("varchar(400)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("DisabledAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("disabled_at");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("disabled_by");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted");

                    b.Property<string>("LogoUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("logo_url");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("modified_at");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("modified_by");

                    b.Property<DateTime?>("RestoredAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("restored_at");

                    b.Property<string>("RestoredBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("restored_by");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique()
                        .HasDatabaseName("brand_title_unique_index");

                    b.ToTable("brands", (string)null);
                });

            modelBuilder.Entity("Entities.Model", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime?>("ActivatedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("activated_at");

                    b.Property<string>("ActivatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("activated_by");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("brand_id");

                    b.Property<byte>("Category")
                        .HasColumnType("tinyint")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("SYSDATETIME()");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasDefaultValue("app_dev")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("varchar(400)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("DisabledAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("disabled_at");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("disabled_by");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("modified_at");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("modified_by");

                    b.Property<string>("PhotoUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("photo_url");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("product_id");

                    b.Property<DateTime?>("RestoredAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("restored_at");

                    b.Property<string>("RestoredBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("restored_by");

                    b.Property<string>("ThumbUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("thumb_url");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique()
                        .HasDatabaseName("model_title_unique_index");

                    b.HasIndex(new[] { "BrandId" }, "brand_id_fk_index");

                    b.HasIndex(new[] { "ProductId" }, "product_id_fk_index");

                    b.ToTable("models", (string)null);
                });

            modelBuilder.Entity("Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime?>("ActivatedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("activated_at");

                    b.Property<string>("ActivatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("activated_by");

                    b.Property<byte>("Category")
                        .HasColumnType("tinyint")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("SYSDATETIME()");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasDefaultValue("app_dev")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("deleted_at");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("deleted_by");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("varchar(400)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("DisabledAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("disabled_at");

                    b.Property<string>("DisabledBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("disabled_by");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("modified_at");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("modified_by");

                    b.Property<DateTime?>("RestoredAt")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("restored_at");

                    b.Property<string>("RestoredBy")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("restored_by");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("title");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique()
                        .HasDatabaseName("product_title_unique_index");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2(3)")
                        .HasColumnName("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)")
                        .HasColumnName("FirstName");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint")
                        .HasColumnName("Gender");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("LastName");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Entities.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UsersRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Entities.Model", b =>
                {
                    b.HasOne("Entities.Brand", "Brand")
                        .WithMany("Models")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("brands_models_fk");

                    b.HasOne("Entities.Product", "Product")
                        .WithMany("Models")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("products_models_fk");

                    b.Navigation("Brand");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Entities.UserRole", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entities.Brand", b =>
                {
                    b.Navigation("Models");
                });

            modelBuilder.Entity("Entities.Product", b =>
                {
                    b.Navigation("Models");
                });

            modelBuilder.Entity("Entities.User", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}