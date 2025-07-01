using Graduation.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.RegularExpressions;
using Group = Graduation.Entities.Group;
using Connection = Graduation.Entities.Connection;

namespace Graduation.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        // ✅ أضفنا هنا جدول الربط
        public DbSet<UserConnection> UserConnections { get; set; }

        public DbSet<Photo> Photos { get; set; }  // أضف DbSet للصور


        public DbSet<Location> Locations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
               .HasMany(ur => ur.UserRoles)
               .WithOne(u => u.User)
               .HasForeignKey(ur => ur.UserId)
               .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<Message>()
               .HasOne(u => u.Recipient)
               .WithMany(m => m.MessagesReceived)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ إعداد العلاقة بين الـ blind و الـ sight
            builder.Entity<UserConnection>()
                .HasOne(c => c.Blind)
                .WithMany()
                .HasForeignKey(c => c.BlindId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserConnection>()
                .HasOne(c => c.Sight)
                .WithMany()
                .HasForeignKey(c => c.SightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Location>()
                .HasOne(l => l.AppUser)
                .WithMany()
                .HasForeignKey(l => l.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Photo>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.Photos)
                .HasForeignKey(p => p.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);  // لو المستخدم اتحذف، الصور تتحذف معاه
        }
    }
}
