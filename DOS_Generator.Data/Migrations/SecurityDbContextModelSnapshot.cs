﻿// <auto-generated />
using System;
using DOS_Generator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DOS_Generator.Data.Migrations
{
    [DbContext(typeof(SecurityDbContext))]
    partial class SecurityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("DOS_Generator.Core.Models.Agency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Fax")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Agencies");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Declaration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("FacilityId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsReceived")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSent")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OfficerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Operation")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PortId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecLevel")
                        .HasColumnType("TEXT");

                    b.Property<int>("ShipId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FacilityId");

                    b.HasIndex("OfficerId");

                    b.HasIndex("PortId");

                    b.HasIndex("ShipId");

                    b.ToTable("Declarations");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Facility", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PortId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PortId");

                    b.ToTable("Facilities");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.MailServer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Host")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServiceName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MailServers");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Officer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Initials")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.Property<string>("TemplatePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Officers");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Port", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Ports");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Ship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AgencyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImoNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PortOfRegistry")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AgencyId");

                    b.ToTable("Ships");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("EmailPassword")
                        .HasColumnType("BLOB");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MailServerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("OfficerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MailServerId");

                    b.HasIndex("OfficerId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Declaration", b =>
                {
                    b.HasOne("DOS_Generator.Core.Models.Facility", "Facility")
                        .WithMany()
                        .HasForeignKey("FacilityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DOS_Generator.Core.Models.Officer", "Officer")
                        .WithMany("Declarations")
                        .HasForeignKey("OfficerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DOS_Generator.Core.Models.Port", "Port")
                        .WithMany()
                        .HasForeignKey("PortId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DOS_Generator.Core.Models.Ship", "Ship")
                        .WithMany()
                        .HasForeignKey("ShipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Facility");

                    b.Navigation("Officer");

                    b.Navigation("Port");

                    b.Navigation("Ship");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Facility", b =>
                {
                    b.HasOne("DOS_Generator.Core.Models.Port", "Port")
                        .WithMany("Facilities")
                        .HasForeignKey("PortId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Port");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Ship", b =>
                {
                    b.HasOne("DOS_Generator.Core.Models.Agency", "Agency")
                        .WithMany("Ships")
                        .HasForeignKey("AgencyId");

                    b.Navigation("Agency");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.User", b =>
                {
                    b.HasOne("DOS_Generator.Core.Models.MailServer", "MailServer")
                        .WithMany()
                        .HasForeignKey("MailServerId");

                    b.HasOne("DOS_Generator.Core.Models.Officer", "Officer")
                        .WithMany()
                        .HasForeignKey("OfficerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MailServer");

                    b.Navigation("Officer");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Agency", b =>
                {
                    b.Navigation("Ships");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Officer", b =>
                {
                    b.Navigation("Declarations");
                });

            modelBuilder.Entity("DOS_Generator.Core.Models.Port", b =>
                {
                    b.Navigation("Facilities");
                });
#pragma warning restore 612, 618
        }
    }
}
