using System.IO;
using System.Runtime.InteropServices;
using SystemPulse.Models;

namespace SystemPulse.Services;

public sealed class SystemMetricsProvider : IDisposable
{
    private readonly GpuCounter _gpuCounter = new();
    private ulong _lastIdle;
    private ulong _lastKernel;
    private ulong _lastUser;

    public SystemMetricsProvider()
    {
        TryReadCpuTimes(out _lastIdle, out _lastKernel, out _lastUser);
    }

    public SystemSnapshot GetSnapshot()
    {
        var cpu = GetCpuPercent();
        var memory = GetMemory();
        var disk = GetPrimaryDisk();
        var gpu = _gpuCounter.TryReadPercent();

        return new SystemSnapshot(
            cpu,
            memory.UsedBytes,
            memory.TotalBytes,
            gpu,
            disk.UsedBytes,
            disk.TotalBytes,
            DateTimeOffset.Now);
    }

    public void Dispose()
    {
        _gpuCounter.Dispose();
    }

    private double GetCpuPercent()
    {
        if (!TryReadCpuTimes(out var idle, out var kernel, out var user))
        {
            return 0;
        }

        var idleDelta = idle - _lastIdle;
        var kernelDelta = kernel - _lastKernel;
        var userDelta = user - _lastUser;
        var totalDelta = kernelDelta + userDelta;

        _lastIdle = idle;
        _lastKernel = kernel;
        _lastUser = user;

        if (totalDelta == 0)
        {
            return 0;
        }

        return Math.Clamp((1.0 - (double)idleDelta / totalDelta) * 100.0, 0, 100);
    }

    private static bool TryReadCpuTimes(out ulong idle, out ulong kernel, out ulong user)
    {
        idle = 0;
        kernel = 0;
        user = 0;

        if (!GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
        {
            return false;
        }

        idle = idleTime.ToUInt64();
        kernel = kernelTime.ToUInt64();
        user = userTime.ToUInt64();
        return true;
    }

    private static MemoryInfo GetMemory()
    {
        var status = new MemoryStatusEx();
        if (!GlobalMemoryStatusEx(status))
        {
            return new MemoryInfo(0, 0);
        }

        return new MemoryInfo(status.TotalPhys - status.AvailPhys, status.TotalPhys);
    }

    private static DiskInfo GetPrimaryDisk()
    {
        var root = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
        try
        {
            var drive = new DriveInfo(root);
            if (!drive.IsReady)
            {
                return new DiskInfo(0, 0);
            }

            return new DiskInfo(drive.TotalSize - drive.AvailableFreeSpace, drive.TotalSize);
        }
        catch (IOException)
        {
            return new DiskInfo(0, 0);
        }
        catch (UnauthorizedAccessException)
        {
            return new DiskInfo(0, 0);
        }
    }

    private readonly record struct MemoryInfo(ulong UsedBytes, ulong TotalBytes);
    private readonly record struct DiskInfo(long UsedBytes, long TotalBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemTimes(out FileTime idleTime, out FileTime kernelTime, out FileTime userTime);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx buffer);

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct FileTime
    {
        private readonly uint _lowDateTime;
        private readonly uint _highDateTime;

        public ulong ToUInt64() => ((ulong)_highDateTime << 32) | _lowDateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    private sealed class MemoryStatusEx
    {
        private readonly uint _length = (uint)Marshal.SizeOf<MemoryStatusEx>();
        private readonly uint _memoryLoad;
        public readonly ulong TotalPhys;
        public readonly ulong AvailPhys;
        private readonly ulong _totalPageFile;
        private readonly ulong _availPageFile;
        private readonly ulong _totalVirtual;
        private readonly ulong _availVirtual;
        private readonly ulong _availExtendedVirtual;
    }

    private sealed class GpuCounter : IDisposable
    {
        private const uint ErrorSuccess = 0;
        private const uint PdhMoreData = 0x800007D2;
        private const uint PdhFmtDouble = 0x00000200;

        private IntPtr _query;
        private IntPtr _counter;
        private bool _available;

        public GpuCounter()
        {
            if (PdhOpenQuery(null, IntPtr.Zero, out _query) != ErrorSuccess)
            {
                _query = IntPtr.Zero;
                return;
            }

            var status = PdhAddEnglishCounter(_query, @"\GPU Engine(*)\Utilization Percentage", IntPtr.Zero, out _counter);
            if (status != ErrorSuccess)
            {
                Dispose();
                return;
            }

            _available = PdhCollectQueryData(_query) == ErrorSuccess;
        }

        public double? TryReadPercent()
        {
            if (!_available || _query == IntPtr.Zero || _counter == IntPtr.Zero)
            {
                return null;
            }

            if (PdhCollectQueryData(_query) != ErrorSuccess)
            {
                return null;
            }

            var status = PdhGetFormattedCounterArray(_counter, PdhFmtDouble, out var bufferSize, out var itemCount, IntPtr.Zero);
            if (status != PdhMoreData || bufferSize == 0 || itemCount == 0)
            {
                return null;
            }

            var buffer = Marshal.AllocHGlobal((int)bufferSize);
            try
            {
                status = PdhGetFormattedCounterArray(_counter, PdhFmtDouble, out bufferSize, out itemCount, buffer);
                if (status != ErrorSuccess || itemCount == 0)
                {
                    return null;
                }

                var total = 0.0;
                var itemSize = Marshal.SizeOf<PdhFmtCounterValueItem>();
                for (var i = 0; i < itemCount; i++)
                {
                    var itemPtr = IntPtr.Add(buffer, i * itemSize);
                    var item = Marshal.PtrToStructure<PdhFmtCounterValueItem>(itemPtr);
                    if (item.Value.CStatus == ErrorSuccess && !double.IsNaN(item.Value.DoubleValue))
                    {
                        total += item.Value.DoubleValue;
                    }
                }

                return Math.Clamp(total, 0, 100);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public void Dispose()
        {
            if (_query != IntPtr.Zero)
            {
                PdhCloseQuery(_query);
            }

            _query = IntPtr.Zero;
            _counter = IntPtr.Zero;
            _available = false;
        }

        [DllImport("pdh.dll", CharSet = CharSet.Unicode)]
        private static extern uint PdhOpenQuery(string? dataSource, IntPtr userData, out IntPtr query);

        [DllImport("pdh.dll", EntryPoint = "PdhAddEnglishCounterW", CharSet = CharSet.Unicode)]
        private static extern uint PdhAddEnglishCounter(IntPtr query, string counterPath, IntPtr userData, out IntPtr counter);

        [DllImport("pdh.dll")]
        private static extern uint PdhCollectQueryData(IntPtr query);

        [DllImport("pdh.dll")]
        private static extern uint PdhCloseQuery(IntPtr query);

        [DllImport("pdh.dll", CharSet = CharSet.Unicode)]
        private static extern uint PdhGetFormattedCounterArray(
            IntPtr counter,
            uint format,
            out uint bufferSize,
            out uint itemCount,
            IntPtr itemBuffer);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct PdhFmtCounterValue
        {
            public readonly uint CStatus;
            public readonly double DoubleValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct PdhFmtCounterValueItem
        {
            public readonly IntPtr Name;
            public readonly PdhFmtCounterValue Value;
        }
    }
}
