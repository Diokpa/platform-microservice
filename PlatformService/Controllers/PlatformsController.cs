using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataservices.Interface;
using PlatformService.Data.Interfaces;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http.Interface;

namespace PlatformService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
      _messageBusClient = messageBusClient;
      _mapper = mapper;
      _commandDataClient = commandDataClient;
      _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      System.Console.WriteLine("--> Getting Platforms....");
      var platformItems = _repository.GetAllPlatforms();
      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
      var platformItem = _repository.GetPlatformById(id);
      if (platformItem != null)
      {
        return Ok(_mapper.Map<PlatformReadDto>(platformItem));
      }

      return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatfrom(PlatformCreateDto platformCreateDto)
    {
      var platfromModel = _mapper.Map<Platform>(platformCreateDto);
      _repository.CreatePlatform(platfromModel);
      _repository.SaveChanges();
      var platformReadDto = _mapper.Map<PlatformReadDto>(platfromModel);

      // Send Sync Message
      try
      {
        await _commandDataClient.SendPlatformToCommand(platformReadDto);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
      }

      // Send Async Message
      try
      {
        var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
        platformPublishedDto.Event = "Platform_Published";
        _messageBusClient.PublishNewPlatform(platformPublishedDto);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send Asynchronously: {ex.Message}");
      }

      return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
  }
}