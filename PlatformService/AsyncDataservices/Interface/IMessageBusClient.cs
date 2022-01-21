using PlatformService.DTOs;

namespace PlatformService.AsyncDataservices.Interface
{
  public interface IMessageBusClient
  {
    void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
  }
}