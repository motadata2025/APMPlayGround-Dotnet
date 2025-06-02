using DotNetCore.CAP;

public class TraceSubscriber : ICapSubscribe
{
    [CapSubscribe("trace.sample")]
    public void HandleTraceSample(dynamic message)
    {
        // Simulate lightweight processing
        Task.Delay(50).Wait();
    }
}