using System;
using System.Collections.Generic;
using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grpc.Interface
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}