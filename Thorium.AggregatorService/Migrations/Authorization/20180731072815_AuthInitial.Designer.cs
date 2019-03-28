﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Thorium.Aggregator.AuthorizationModule;

namespace Thorium.Aggregator.Migrations.Authorization
{
    [DbContext(typeof(AuthorizationDbContext))]
    [Migration("20180731072815_AuthInitial")]
    partial class AuthInitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.AuthorizationNamespace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Key");

                    b.HasKey("Id");

                    b.ToTable("AuthorizationNamespaces");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("OwnerScoped");

                    b.Property<int>("PermissionType");

                    b.Property<int>("ResourceDetailId");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Key");

                    b.HasKey("Id");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.ResourceDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Key");

                    b.Property<int>("ResourceId");

                    b.HasKey("Id");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceDetails");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.ResourceNamespace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AuthorizationNamespaceId");

                    b.Property<int>("NamespaceId");

                    b.Property<int>("ResourceId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationNamespaceId");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceNamespaces");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.ResourceOwner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ResourceId");

                    b.Property<string>("SpecificResourceId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ResourceOwners");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AuthorizationNamespaceId");

                    b.Property<string>("Description");

                    b.Property<int>("NamespaceId");

                    b.Property<int?>("ParentRoleId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationNamespaceId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RoleId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.Permission", b =>
                {
                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.ResourceDetail", b =>
                {
                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.Resource", "Resource")
                        .WithMany("ResourceDetails")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.ResourceNamespace", b =>
                {
                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.AuthorizationNamespace", "AuthorizationNamespace")
                        .WithMany()
                        .HasForeignKey("AuthorizationNamespaceId");

                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.Role", b =>
                {
                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.AuthorizationNamespace")
                        .WithMany("Roles")
                        .HasForeignKey("AuthorizationNamespaceId");
                });

            modelBuilder.Entity("StsServerIdentity.AuthorizationModule.Models.UserRole", b =>
                {
                    b.HasOne("StsServerIdentity.AuthorizationModule.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}