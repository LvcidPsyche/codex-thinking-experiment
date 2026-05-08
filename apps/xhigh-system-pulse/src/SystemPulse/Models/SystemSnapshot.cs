namespace SystemPulse.Models;

public sealed record SystemSnapshot(
    double CpuPercent,
    ulong MemoryUsedBytes,
    ulong MemoryTotalBytes,
    double? GpuPercent,
    long DiskUsedBytes,
    long DiskTotalBytes,
    DateTimeOffset CapturedAt)
{
    public bool HasGpu => GpuPercent.HasValue;
}
