using System;
using System.Collections.Generic;
using CommandsService.Data.Interface;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
  public class PrepDb
  {
    public static void Prepopulation(IApplicationBuilder applicationBuilder)
    {
      using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();


        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
      }
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
      Console.WriteLine("Seeding new Platforms...");
      foreach (var platform in platforms)
      {
        if (!repository.ExternalPlatformExists(platform.ExternalID))
        {
          repository.CreatePlatform(platform);
        }
        repository.SaveChanges();
      }
    }
  }
}