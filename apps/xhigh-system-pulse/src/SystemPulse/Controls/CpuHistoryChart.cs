using System.Windows;
using System.Windows.Media;

namespace SystemPulse.Controls;

public sealed class CpuHistoryChart : FrameworkElement
{
    public static readonly DependencyProperty SamplesProperty =
        DependencyProperty.Register(
            nameof(Samples),
            typeof(IReadOnlyList<double>),
            typeof(CpuHistoryChart),
            new FrameworkPropertyMetadata(Array.Empty<double>(), FrameworkPropertyMetadataOptions.AffectsRender));

    public IReadOnlyList<double> Samples
    {
        get => (IReadOnlyList<double>)GetValue(SamplesProperty);
        set => SetValue(SamplesProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        var background = new SolidColorBrush(Color.FromRgb(24, 29, 38));
        var gridPen = new Pen(new SolidColorBrush(Color.FromArgb(45, 255, 255, 255)), 1);
        var linePen = new Pen(new SolidColorBrush(Color.FromRgb(64, 201, 255)), 2.2)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        drawingContext.DrawRoundedRectangle(background, null, bounds, 8, 8);

        for (var i = 1; i < 4; i++)
        {
            var y = bounds.Height * i / 4;
            drawingContext.DrawLine(gridPen, new Point(0, y), new Point(bounds.Width, y));
        }

        var samples = Samples;
        if (samples.Count < 2)
        {
            return;
        }

        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            for (var i = 0; i < samples.Count; i++)
            {
                var x = samples.Count == 1 ? 0 : i * bounds.Width / (samples.Count - 1);
                var value = Math.Clamp(samples[i], 0, 100);
                var y = bounds.Height - value * bounds.Height / 100;
                var point = new Point(x, y);

                if (i == 0)
                {
                    context.BeginFigure(point, false, false);
                }
                else
                {
                    context.LineTo(point, true, false);
                }
            }
        }

        geometry.Freeze();
        drawingContext.DrawGeometry(null, linePen, geometry);
    }
}
