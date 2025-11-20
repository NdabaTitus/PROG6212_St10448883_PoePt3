using System;
using System.Threading.Tasks;
using Poe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poe.Data;


namespace Poe.Data
{

    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Get required services from the DI container.
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 
            context.Database.Migrate();

            // 
            string[] roles = { "HR", "Worker", "ProjectManager", "ConstructionManager" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 
            var hrEmail = "hr@site.com";
            var hrUser = await userManager.FindByEmailAsync(hrEmail);
            if (hrUser == null)
            {
                hrUser = new IdentityUser
                {
                    UserName = hrEmail,
                    Email = hrEmail,
                    EmailConfirmed = true
                };

                // 
                await userManager.CreateAsync(hrUser, "Hr@123!");

                await userManager.AddToRoleAsync(hrUser, "HR");

                context.EmployeeProfiles.Add(new EmployeeProfile
                {
                    UserId = hrUser.Id,
                    Name = "HR",
                    Surname = "Manager",
                    Department = "Admin",
                    DefaultRatePerJob = 0,
                    RoleName = "HR"
                });

                await context.SaveChangesAsync();
            }

            // 
            var pmEmail = "pm@site.com";
            var pmUser = await userManager.FindByEmailAsync(pmEmail);
            if (pmUser == null)
            {
                pmUser = new IdentityUser
                {
                    UserName = pmEmail,
                    Email = pmEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(pmUser, "Pm@123!");
                await userManager.AddToRoleAsync(pmUser, "ProjectManager");

                context.EmployeeProfiles.Add(new EmployeeProfile
                {
                    UserId = pmUser.Id,
                    Name = "Project",
                    Surname = "Manager",
                    Department = "Projects",
                    DefaultRatePerJob = 0,
                    RoleName = "ProjectManager"
                });

                await context.SaveChangesAsync();
            }

            // 
            var cmEmail = "cm@site.com";
            var cmUser = await userManager.FindByEmailAsync(cmEmail);
            if (cmUser == null)
            {
                cmUser = new IdentityUser
                {
                    UserName = cmEmail,
                    Email = cmEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(cmUser, "Cm@123!");
                await userManager.AddToRoleAsync(cmUser, "ConstructionManager");

                context.EmployeeProfiles.Add(new EmployeeProfile
                {
                    UserId = cmUser.Id,
                    Name = "Construction",
                    Surname = "Manager",
                    Department = "Construction",
                    DefaultRatePerJob = 0,
                    RoleName = "ConstructionManager"
                });

                await context.SaveChangesAsync();
            }

            // 
            var workerEmail = "worker@site.com";
            var workerUser = await userManager.FindByEmailAsync(workerEmail);
            if (workerUser == null)
            {
                workerUser = new IdentityUser
                {
                    UserName = workerEmail,
                    Email = workerEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(workerUser, "Worker@123!");
                await userManager.AddToRoleAsync(workerUser, "Worker");

                context.EmployeeProfiles.Add(new EmployeeProfile
                {
                    UserId = workerUser.Id,
                    Name = "John",
                    Surname = "Builder",
                    Department = "Masonry",
                    DefaultRatePerJob = 200,
                    RoleName = "Worker"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}