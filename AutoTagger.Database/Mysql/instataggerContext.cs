using System;
using Microsoft.EntityFrameworkCore;

namespace AutoTagger.Database.Mysql
{
    public partial class InstataggerContext : DbContext
    {
        public virtual DbSet<Debug> Debug { get; set; }
        public virtual DbSet<Itags> Itags { get; set; }
        public virtual DbSet<Mtags> Mtags { get; set; }
        public virtual DbSet<PhotoItagRel> PhotoItagRel { get; set; }
        public virtual DbSet<Photos> Photos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ip = Environment.GetEnvironmentVariable("instatagger_mysql_ip");
                var user = Environment.GetEnvironmentVariable("instatagger_mysql_user");
                var pw = Environment.GetEnvironmentVariable("instatagger_mysql_pw");
                var db = Environment.GetEnvironmentVariable("instatagger_mysql_db");
                optionsBuilder.UseMySql($"Server={ip};User Id={user};Password={pw};Database={db}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Debug>(entity =>
            {
                entity.ToTable("debug");

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("text");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasColumnName("source")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Itags>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Name });

                entity.ToTable("itags");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(30);

                entity.Property(e => e.Posts)
                    .HasColumnName("posts")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Mtags>(entity =>
            {
                entity.ToTable("mtags");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.HasIndex(e => e.PhotoId)
                    .HasName("photoId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(30);

                entity.Property(e => e.PhotoId)
                    .HasColumnName("photoId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("float(11)");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasColumnName("source")
                    .HasMaxLength(30);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.Mtags)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("mtags_ibfk_1");
            });

            modelBuilder.Entity<PhotoItagRel>(entity =>
            {
                entity.ToTable("photo_itag_rel");

                entity.HasIndex(e => e.ItagId)
                    .HasName("itagId");

                entity.HasIndex(e => e.PhotoId)
                    .HasName("photoId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ItagId)
                    .HasColumnName("itagId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoId)
                    .HasColumnName("photoId")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Itag)
                    .WithMany(p => p.PhotoItagRel)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.ItagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("photo_itag_rel_ibfk_2");

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.PhotoItagRel)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.PhotoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("photo_itag_rel_ibfk_1");
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

                entity.Property(e => e.Following)
                    .HasColumnName("following")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LargeUrl)
                    .IsRequired()
                    .HasColumnName("largeUrl")
                    .HasColumnType("text");

                entity.Property(e => e.Likes)
                    .HasColumnName("likes")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Posts)
                    .HasColumnName("posts")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ThumbUrl)
                    .IsRequired()
                    .HasColumnName("thumbUrl")
                    .HasColumnType("text");

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
