using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserMgt.DAL.Entities;

namespace UserMgt.DAL.Context
{
    public class UserDbContext : IdentityDbContext<AppUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(e =>
            {
                e.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired()
                .HasAnnotation("ErrorMessage", "Email field is required");
                e.HasIndex(u => u.Email, "IX_UniqueEmail")
                 .IsUnique();
            });
        }
    }
}
