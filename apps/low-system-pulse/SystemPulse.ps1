Add-Type -AssemblyName PresentationFramework
Add-Type -AssemblyName PresentationCore
Add-Type -AssemblyName WindowsBase
Add-Type -AssemblyName Microsoft.VisualBasic

$ErrorActionPreference = "Stop"

function Format-Bytes {
    param([double]$Bytes)
    if ($Bytes -ge 1TB) { return "{0:N1} TB" -f ($Bytes / 1TB) }
    if ($Bytes -ge 1GB) { return "{0:N1} GB" -f ($Bytes / 1GB) }
    if ($Bytes -ge 1MB) { return "{0:N1} MB" -f ($Bytes / 1MB) }
    return "{0:N0} B" -f $Bytes
}

function Get-GpuUsage {
    if (-not $script:GpuCounters -or $script:GpuCounters.Count -eq 0) { return $null }
    $total = 0
    foreach ($counter in $script:GpuCounters) {
        try { $total += $counter.NextValue() } catch { }
    }
    return [math]::Min(100, [math]::Max(0, [math]::Round($total, 0)))
}

function Get-SystemMetrics {
    $cpu = try {
        [math]::Round($script:CpuCounter.NextValue(), 0)
    }
    catch {
        0
    }

    $computerInfo = [Microsoft.VisualBasic.Devices.ComputerInfo]::new()
    $totalRam = [double]$computerInfo.TotalPhysicalMemory
    $freeRam = [double]$computerInfo.AvailablePhysicalMemory
    $usedRam = $totalRam - $freeRam

    $disk = [System.IO.DriveInfo]::GetDrives() |
        Where-Object { $_.IsReady -and $_.Name -eq "C:\" } |
        Select-Object -First 1
    if (-not $disk) {
        $disk = [System.IO.DriveInfo]::GetDrives() |
            Where-Object { $_.IsReady -and $_.DriveType -eq [System.IO.DriveType]::Fixed } |
            Select-Object -First 1
    }
    $diskTotal = [double]$disk.TotalSize
    $diskFree = [double]$disk.AvailableFreeSpace
    $diskUsed = $diskTotal - $diskFree

    [pscustomobject]@{
        Cpu = [math]::Min(100, [math]::Max(0, $cpu))
        Gpu = Get-GpuUsage
        RamUsed = $usedRam
        RamTotal = $totalRam
        DiskUsed = $diskUsed
        DiskTotal = $diskTotal
    }
}

$xaml = @"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="System Pulse"
        Width="330"
        Height="314"
        ResizeMode="NoResize"
        WindowStartupLocation="Manual"
        Topmost="True"
        ShowInTaskbar="False"
        Background="#101216"
        Foreground="#EEF2F5"
        FontFamily="Segoe UI">
    <Border Background="#101216" BorderBrush="#2C333D" BorderThickness="1" CornerRadius="8" Padding="14">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>
            <DockPanel Grid.Row="0" LastChildFill="False">
                <TextBlock Text="System Pulse" FontSize="15" FontWeight="SemiBold" DockPanel.Dock="Left"/>
                <TextBlock x:Name="UpdatedText" Text="starting" Foreground="#8E99A8" FontSize="11" DockPanel.Dock="Right" Margin="0,2,0,0"/>
            </DockPanel>
            <Grid Grid.Row="1" Margin="0,12,0,12">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Border Background="#171B21" BorderBrush="#2B323C" BorderThickness="1" CornerRadius="6" Padding="10" Margin="0,0,6,0">
                    <StackPanel>
                        <TextBlock Text="CPU" Foreground="#8E99A8" FontSize="11"/>
                        <TextBlock x:Name="CpuText" Text="--%" FontSize="34" FontWeight="SemiBold" Foreground="#69D2E7"/>
                    </StackPanel>
                </Border>
                <Border Grid.Column="1" Background="#171B21" BorderBrush="#2B323C" BorderThickness="1" CornerRadius="6" Padding="10" Margin="6,0,0,0">
                    <StackPanel>
                        <TextBlock Text="GPU" Foreground="#8E99A8" FontSize="11"/>
                        <TextBlock x:Name="GpuText" Text="checking" FontSize="20" FontWeight="SemiBold" Foreground="#F6C85F" TextWrapping="Wrap"/>
                    </StackPanel>
                </Border>
            </Grid>
            <Border Grid.Row="2" Background="#171B21" BorderBrush="#2B323C" BorderThickness="1" CornerRadius="6" Padding="8">
                <Grid>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="*"/>
                    </Grid.RowDefinitions>
                    <TextBlock Text="CPU history - 60 seconds" Foreground="#8E99A8" FontSize="11"/>
                    <Canvas x:Name="ChartCanvas" Grid.Row="1" Height="58" Margin="0,8,0,0"/>
                </Grid>
            </Border>
            <Grid Grid.Row="3" Margin="0,12,0,0">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <StackPanel>
                    <TextBlock Text="RAM" Foreground="#8E99A8" FontSize="11"/>
                    <TextBlock x:Name="RamText" Text="-- / --" FontSize="13"/>
                </StackPanel>
                <StackPanel Grid.Column="1">
                    <TextBlock Text="Disk" Foreground="#8E99A8" FontSize="11"/>
                    <TextBlock x:Name="DiskText" Text="-- / --" FontSize="13"/>
                </StackPanel>
            </Grid>
        </Grid>
    </Border>
</Window>
"@

$reader = New-Object System.Xml.XmlNodeReader ([xml]$xaml)
$window = [Windows.Markup.XamlReader]::Load($reader)
$window.Left = [System.Windows.SystemParameters]::WorkArea.Right - $window.Width - 18
$window.Top = [System.Windows.SystemParameters]::WorkArea.Bottom - $window.Height - 18

$script:CpuCounter = [System.Diagnostics.PerformanceCounter]::new("Processor", "% Processor Time", "_Total")
[void]$script:CpuCounter.NextValue()

$script:GpuCounters = New-Object 'System.Collections.Generic.List[System.Diagnostics.PerformanceCounter]'
try {
    $gpuCategory = [System.Diagnostics.PerformanceCounterCategory]::new("GPU Engine")
    foreach ($instance in ($gpuCategory.GetInstanceNames() | Where-Object { $_ -match "engtype_(3D|Compute)" } | Select-Object -First 32)) {
        if ($instance -match "engtype_(3D|Compute)") {
            $counter = [System.Diagnostics.PerformanceCounter]::new("GPU Engine", "Utilization Percentage", $instance)
            [void]$counter.NextValue()
            $script:GpuCounters.Add($counter)
        }
    }
}
catch {
    $script:GpuCounters.Clear()
}

$cpuText = $window.FindName("CpuText")
$gpuText = $window.FindName("GpuText")
$ramText = $window.FindName("RamText")
$diskText = $window.FindName("DiskText")
$updatedText = $window.FindName("UpdatedText")
$chartCanvas = $window.FindName("ChartCanvas")
$history = New-Object 'System.Collections.Generic.Queue[double]'

function Draw-Chart {
    $chartCanvas.Children.Clear()
    $values = @($history.ToArray())
    if ($values.Count -lt 2) { return }

    $width = [math]::Max(1, $chartCanvas.ActualWidth)
    if ($width -le 1) { $width = 280 }
    $height = [math]::Max(1, $chartCanvas.ActualHeight)
    if ($height -le 1) { $height = 58 }

    $line = New-Object System.Windows.Shapes.Polyline
    $line.Stroke = [System.Windows.Media.Brushes]::Cyan
    $line.StrokeThickness = 2
    $line.SnapsToDevicePixels = $true

    for ($i = 0; $i -lt $values.Count; $i++) {
        $x = if ($values.Count -eq 1) { 0 } else { ($i / ($values.Count - 1)) * $width }
        $y = $height - (($values[$i] / 100) * $height)
        [void]$line.Points.Add((New-Object System.Windows.Point($x, $y)))
    }
    [void]$chartCanvas.Children.Add($line)
}

function Refresh-Metrics {
    $metrics = Get-SystemMetrics

    $cpuText.Text = "$($metrics.Cpu)%"
    if ($null -eq $metrics.Gpu) {
        $gpuText.Text = "GPU not detected"
        $gpuText.FontSize = 16
    }
    else {
        $gpuText.Text = "$($metrics.Gpu)%"
        $gpuText.FontSize = 34
    }
    $ramText.Text = "$(Format-Bytes $metrics.RamUsed) / $(Format-Bytes $metrics.RamTotal)"
    $diskText.Text = "$(Format-Bytes $metrics.DiskUsed) / $(Format-Bytes $metrics.DiskTotal)"
    $updatedText.Text = (Get-Date).ToString("HH:mm:ss")

    $history.Enqueue([double]$metrics.Cpu)
    while ($history.Count -gt 60) { [void]$history.Dequeue() }
    Draw-Chart
}

$timer = New-Object System.Windows.Threading.DispatcherTimer
$timer.Interval = [TimeSpan]::FromSeconds(1)
$timer.Add_Tick({ Refresh-Metrics })
$window.Add_Loaded({ $timer.Start() })
$window.Add_Closed({ $timer.Stop() })

[void]$window.ShowDialog()
