﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Quickpaste.Services.BlobServices.Local;
using System;

namespace Quickpaste.Migrations.BlobDb
{
    [DbContext(typeof(BlobDbContext))]
    partial class BlobDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Quickpaste.Services.BlobServices.Local.LocalBlob", b =>
                {
                    b.Property<string>("BlobName")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("BlobData")
                        .IsRequired();

                    b.Property<string>("ContentType")
                        .IsRequired();

                    b.Property<bool>("IsPublic");

                    b.HasKey("BlobName");

                    b.ToTable("Blobs");
                });
#pragma warning restore 612, 618
        }
    }
}
