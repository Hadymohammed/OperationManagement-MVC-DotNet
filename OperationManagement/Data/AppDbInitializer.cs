﻿using Microsoft.AspNetCore.Identity;
using OperationManagement.Data.Static;
using OperationManagement.Models;
using System;

namespace OperationManagement.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDBContext>();
                context.Database.EnsureCreated();
                /*if (!context.Enterprises.Any())
                {
                    context.Enterprises.AddRange(new List<Enterprise>()
                    {
                        new Enterprise
                        {
                            Name="Abdelrahman Elhawary",
                            Accepted=true
                        }
                    });
                    context.SaveChanges();
                }*/
            }
        }
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "operatobusiness@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FirstName="Super",
                        LastName="Admin",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        ProfilePictureURL=Consts.profileImgUrl,
                        Registered=true
                    };
                    await userManager.CreateAsync(newAdminUser, "~1!b-K8qm**s.!7Jg1Gt");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }


                /*
                 string appUserEmail = "user@operation.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FirstName = "Abdelhady",
                        LastName = "Mohmaed",
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        ProfilePictureURL = Consts.profileImgUrl,
                        EnterpriseId = 1,
                        Registered = true
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
                */
            }
        }
    }
}
