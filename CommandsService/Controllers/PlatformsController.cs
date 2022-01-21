using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data.Interface;
using CommandsService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/c/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;
    public PlatformsController(ICommandRepository repository, IMapper mapper)
    {
      _mapper = mapper;
      _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      Console.WriteLine("--> Getting Platforms from CommandService");
      var platforms = _repository.GetAllPlatforms();
      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
      Console.WriteLine("--> Inbound Post # command Service");
      return Ok("Inbound test of from platform controller");
    }
  }
}