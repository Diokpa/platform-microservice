using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;
using CommandsService.SyncDataServices.Grpc.Interface;
using System.Net.Http;

namespace CommandsService.SyncDataServices.Grpc.Implementation
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
      _mapper = mapper;
      _configuration = configuration;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");
      var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
      var client = new GrpcPlatform.GrpcPlatformClient(channel);
      var request = new GetAllRequest();

      try
      {
        var reply = client.GetAllPlatforms(request);
        return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not call GRPC call Server {ex.Message}");
        return null;
      }
    }
  }
}