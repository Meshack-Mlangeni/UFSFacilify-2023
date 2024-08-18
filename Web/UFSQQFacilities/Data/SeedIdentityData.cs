using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public static class SeedIdentityData
    {
        private static readonly string username = "admin";
        private static readonly string password = "@Meshack123";
        private static readonly string adminFname = "Mncedisi";
        private static readonly string adminLname = "Mlangeni";
        private static readonly string adminEmail = "ufsfacilify@ufs.ac.za";
        private static readonly string[] roles = { "Administrator", "User" };
        private static readonly IList<User> preusers = new List<User>
        {
          new User() { UserName = "user1", Email = "user1@ufs4life.ac.za", MobilePassword="@Meshack123" , FirstName = "Thembalethu", LastName = "Mlangeni", DateJoined = DateTime.Now, StudentStaffNumber="2012456789"   },
          new User() { UserName = "user2", Email = "user2@ufs4life.ac.za", MobilePassword="@Meshack123" , FirstName = "France", LastName = "Smith", DateJoined = DateTime.Now, StudentStaffNumber="2015789456"   },
          new User() { UserName = "user3", Email = "user3@ufs4life.ac.za", MobilePassword="@Meshack123" , FirstName = "Bheki", LastName = "Sthole", DateJoined = DateTime.Now, StudentStaffNumber="2012473632"   },
        };

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            AppIdentityDbContext context = app.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

            UserManager<User> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await userManager.FindByNameAsync(username) == null)
            {

                createRoles(roles, roleManager);

                User user = new()
                {
                    UserName = username,
                    FirstName = adminFname,
                    LastName = adminLname,
                    Email = adminEmail,
                    MobilePassword = "@Meshack123"
                };
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, roles[0]);

                addTestUsers(preusers, userManager, password);
            }
            //ModifyMobileUsers(userManager);
        }

        private async static void addTestUsers(IList<User> preusers, UserManager<User> userManager, string password)
        {
            foreach (var user in preusers)
            {
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, roles[1]);
            }
        }

        private async static void createRoles(string[] roles, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
                if (await roleManager.FindByNameAsync(role) == null)
                    await roleManager.CreateAsync(new(role));
        }
    }
}