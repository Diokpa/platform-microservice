using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
      using (var servoceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(servoceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
      }
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
      if (isProd)
      {
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
      }
      if (!context.Platforms.Any())
      {
        Console.WriteLine("--> Seeding Data ");
        context.Platforms.AddRange(
            new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
            new Platform() { Name = "SQl Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );
        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("--> We already have data");
      }
    }
  }
}