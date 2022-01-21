namespace CommandService.EventProcessing.Interface
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}