﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoTagger.Database
{
    using AutoTagger.Database.Mysql;

    public partial class InstataggerContext : DbContext
    {
        public virtual DbSet<Itags> Itags { get; set; }
        public virtual DbSet<Mtags> Mtags { get; set; }
        public virtual DbSet<Photos> Photos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var pw = "";
                optionsBuilder.UseMySql($"Server=78.46.178.185;User Id=InstaTagger;Password={pw};Database=instatagger");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Itags>(entity =>
            {
                entity.ToTable("itags");

                entity.HasIndex(e => e.PhotoId)
                    .HasName("photoId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoId)
                    .HasColumnName("photoId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(30);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.Itags)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("itags_ibfk_1");
            });

            modelBuilder.Entity<Mtags>(entity =>
            {
                entity.ToTable("mtags");

                entity.HasIndex(e => e.PhotoId)
                    .HasName("photoId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoId)
                    .HasColumnName("photoId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(30);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.Mtags)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("mtags_ibfk_1");
            });

            modelBuilder.Entity<Photos>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Shortcode });

                entity.ToTable("photos");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.HasIndex(e => e.Shortcode)
                    .HasName("imgId")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Shortcode)
                    .HasColumnName("shortcode")
                    .HasMaxLength(50);

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Follower)
                    .HasColumnName("follower")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasColumnType("text");

                entity.Property(e => e.Likes)
                    .HasColumnName("likes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Uploaded)
                    .HasColumnName("uploaded")
                    .HasColumnType("timestamp");

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasColumnName("user")
                    .HasMaxLength(50);
            });
        }
    }
}
