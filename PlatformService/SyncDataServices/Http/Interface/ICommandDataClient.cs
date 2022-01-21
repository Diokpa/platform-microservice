using System.Threading.Tasks;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http.Interface
{
  public interface ICommandDataClient
  {
    Task SendPlatformToCommand(PlatformReadDto plat);
  }
}
