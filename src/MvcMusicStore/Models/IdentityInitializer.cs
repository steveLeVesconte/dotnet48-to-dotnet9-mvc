using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace MvcMusicStore.Models
{
    public class IdentityInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Create Admin role
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            // Create Visitor role
            if (!roleManager.RoleExists("Visitor"))
            {
                roleManager.Create(new IdentityRole("Visitor"));
            }

            // Create default admin user
            var adminUser = userManager.FindByName("admin@musicstore.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@musicstore.com",
                    Email = "admin@musicstore.com",
                    Role = "Admin"
                };

                var result = userManager.Create(user, "Admin123!");
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            base.Seed(context);
        }
    }
}