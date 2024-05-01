using System.Security.Claims;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (userMgr.Users.Any())
            {
                return;
            }
            var will = userMgr.FindByNameAsync("will").Result;
            if (will == null)
            {
                will = new ApplicationUser
                {
                    UserName = "will",
                    Email = "will@gmail.com",
                };
                var result = userMgr.CreateAsync(will, "Will123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(will, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Will Zr"),
                            new Claim(JwtClaimTypes.GivenName, "Will"),
                            new Claim(JwtClaimTypes.FamilyName, "Zimmermann"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("will created");
            }
            else
            {
                Log.Debug("will already exists");
            }
        }
    }
}
