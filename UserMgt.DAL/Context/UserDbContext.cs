using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserMgt.DAL.Entities;

namespace UserMgt.DAL.Context
{
    public class UserDbContext : IdentityDbContext<AppUser>
    {
        public UserDbContext() : base()
        {
        }
        public UserDbContext(DbContextOptions options) : base(options)
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
