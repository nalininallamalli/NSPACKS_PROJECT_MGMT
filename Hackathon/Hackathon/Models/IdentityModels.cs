using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /*
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("MyUsers");

             modelBuilder.Entity<ApplicationUser>()
            .ToTable("User", "dbo").Property(p => p.Id).HasColumnName("UserId");

            modelBuilder.Entity<ApplicationUser>()
            .ToTable("User", "dbo").HasKey(key => key.Id);

            modelBuilder.Entity<ApplicationUser>()
            .ToTable("User", "dbo").Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ApplicationUser>()
            .ToTable("User", "dbo").Property(p => p.UserName).HasColumnName("UserName");

            modelBuilder.Entity<IdentityUser>()
            .ToTable("User", "dbo").Property(p => p.PasswordHash).HasColumnName("Password");

            modelBuilder.Entity<IdentityUser>()
            .ToTable("User", "dbo").Property(p => p.Email).HasColumnName("EmailID");

            modelBuilder.Entity<IdentityUser>()
            .ToTable("User", "dbo").Ignore(p => p.AccessFailedCount);

            
/*
            modelBuilder.Entity<IdentityUser>()
            .Ignore(p => p.Roles);

            modelBuilder.Entity<IdentityUser>()
            .Ignore(p => p.Claims);

            modelBuilder.Entity<IdentityUser>()
            .Ignore(p => p.Logins);
* /
            /*
            modelBuilder.Entity<IdentityUser>().ToTable("MyUsers").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<ApplicationUser>().ToTable("MyUsers").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<IdentityUserRole>().ToTable("MyUserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("MyUserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("MyUserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("MyRoles");
            * /
        }
         */
    }
}