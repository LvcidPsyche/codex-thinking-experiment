using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SystemPulse.Models;
using SystemPulse.Services;

namespace SystemPulse;

public partial class MainWindow : Window
{
    private readonly SystemMetricsProvider _metricsProvider = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly Queue<double> _cpuHistory = new();
    private bool _sampling;

    public MainWindow()
    {
        InitializeComponent();
        _timer.Tick += async (_, _) => await RefreshAsync();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        PinToBottomRight();
        await RefreshAsync();
        _timer.Start();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _timer.Stop();
        _metricsProvider.Dispose();
    }

    private async Task RefreshAsync()
    {
        if (_sampling)
        {
            return;
        }

        _sampling = true;
        try
        {
            var snapshot = await Task.Run(_metricsProvider.GetSnapshot);
            Render(snapshot);
        }
        catch (Exception ex)
        {
            UpdatedText.Text = $"Metrics unavailable: {ex.Message}";
        }
        finally
        {
            _sampling = false;
        }
    }

    private void Render(SystemSnapshot snapshot)
    {
        PushCpuSample(snapshot.CpuPercent);

        CpuValueText.Text = $"{snapshot.CpuPercent:0}%";
        CpuChart.Samples = _cpuHistory.ToArray();

        MemoryText.Text = $"{FormatBytes(snapshot.MemoryUsedBytes)} / {FormatBytes(snapshot.MemoryTotalBytes)}";
        MemoryBar.Value = Percent(snapshot.MemoryUsedBytes, snapshot.MemoryTotalBytes);

        if (snapshot.HasGpu)
        {
            GpuText.Text = $"{snapshot.GpuPercent!.Value:0}%";
            GpuBar.Value = snapshot.GpuPercent.Value;
            GpuBar.Opacity = 1;
        }
        else
        {
            GpuText.Text = "GPU not detected";
            GpuBar.Value = 0;
            GpuBar.Opacity = 0.3;
        }

        DiskText.Text = $"{FormatBytes(snapshot.DiskUsedBytes)} / {FormatBytes(snapshot.DiskTotalBytes)}";
        DiskBar.Value = Percent(snapshot.DiskUsedBytes, snapshot.DiskTotalBytes);
        UpdatedText.Text = $"Updated {snapshot.CapturedAt:HH:mm:ss}";
    }

    private void PushCpuSample(double cpuPercent)
    {
        _cpuHistory.Enqueue(cpuPercent);
        while (_cpuHistory.Count > 60)
        {
            _cpuHistory.Dequeue();
        }
    }

    private void PinToBottomRight()
    {
        var workArea = SystemParameters.WorkArea;
        Left = Math.Max(12, workArea.Right - Width - 24);
        Top = Math.Max(12, workArea.Bottom - Height - 24);
    }

    private void Shell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private static double Percent(ulong used, ulong total)
    {
        if (total == 0)
        {
            return 0;
        }

        return Math.Clamp((double)used / total * 100, 0, 100);
    }

    private static double Percent(long used, long total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Clamp((double)used / total * 100, 0, 100);
    }

    private static string FormatBytes(ulong bytes) => FormatBytes((double)bytes);

    private static string FormatBytes(long bytes) => FormatBytes((double)Math.Max(0, bytes));

    private static string FormatBytes(double bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        var unit = 0;
        while (bytes >= 1024 && unit < units.Length - 1)
        {
            bytes /= 1024;
            unit++;
        }

        return unit < 3 ? $"{bytes:0} {units[unit]}" : $"{bytes:0.0} {units[unit]}";
    }
}
