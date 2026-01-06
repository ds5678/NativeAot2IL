using NativeAot2IL.Logging;

namespace NativeAot2IL;

public class TimingScope : IDisposable
{
    private readonly DateTime StartTime;
    private readonly string Name;
    
    public TimingScope(string name)
    {
        Name = name;
        StartTime = DateTime.UtcNow;
    }
    
    public void Dispose()
    {
        var endTime = DateTime.UtcNow;
        var duration = endTime - StartTime;
        Logger.InfoNewline($"{Name} in {duration.TotalMilliseconds}ms", "Main");
    }
}