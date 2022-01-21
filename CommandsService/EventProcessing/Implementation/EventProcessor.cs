using System;
using System.Text.Json;
using AutoMapper;
using CommandService.EventProcessing.Interface;
using CommandsService.Data.Interface;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing.Implementation
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;
    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;

    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);

      switch (eventType)
      {
        case EventType.PlatformPublished:
          AddPlatform(message);
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determine Event");
      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Platform_Published":
          Console.WriteLine("--> Platform published Event Detected");
          return EventType.PlatformPublished;
        default:
          Console.WriteLine("--> Could not determine the event type");
          return EventType.Undetermined;
      }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
          var platform = _mapper.Map<Platform>(platformPublishedDto);
          if (!repository.ExternalPlatformExists(platform.ExternalID))
          {
            repository.CreatePlatform(platform);
            repository.SaveChanges();
            Console.WriteLine($"--> Platform added!");
          }
          else
          {
            Console.WriteLine("--> Platform already exists");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
        }
      }

    }
  }

  enum EventType
  {
    PlatformPublished,
    Undetermined
  }
}