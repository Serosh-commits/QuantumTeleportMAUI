using Microcharts;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using QuantumLib;
using QuantumTeleportMAUI.Models;
using QuantumTeleportMAUI.Services;

namespace QuantumTeleportMAUI.Views;

public partial class MainPage : ContentPage {
    private readonly QuantumService _qService;
    private readonly DataService _dataSvc;
    private List<ExperimentResult> _runHistory = new();
    private double _animAngle = 0;

    public MainPage(QuantumService qService, DataService dataService) {
        InitializeComponent();
        _qService = qService;
        _dataSvc = dataService;
        _ = RefreshHistoryAsync();

        thetaControl.ValueChanged += (s, e) => {
            thetaLabel.Text = $"theta: {e.NewValue:F3} ({e.NewValue * 180 / Math.PI:F0}°)";
            blochView.InvalidateSurface();
        };
        phiControl.ValueChanged += (s, e) => {
            phiLabel.Text = $"phi: {e.NewValue:F3} ({e.NewValue * 180 / Math.PI:F0}°)";
            blochView.InvalidateSurface();
        };
        shotsControl.ValueChanged += (s, e) => shotsLabel.Text = $"shots: {e.NewValue}";
        noiseControl.ValueChanged += (s, e) => noiseLabel.Text = $"noise: {e.NewValue:P0}";
    }

    private async void RunSim(object sender, EventArgs e) {
        statusText.Text = "teleporting...";
        var settings = new TeleportRequest {
            NumQubits = 1,
            Theta = thetaControl.Value,
            Phi = phiControl.Value,
            Shots = (int)shotsControl.Value,
            Noise = noiseControl.Value,
            EnableEC = ecCheck.IsChecked
        };

        try {
            var success = await _qService.SimulateTeleportAsync(settings);
            var newRun = new ExperimentResult {
                SuccessRate = success,
                Fidelity = success,
                NumQubits = settings.NumQubits,
                Theta = settings.Theta,
                Phi = settings.Phi,
                Shots = settings.Shots,
                Noise = settings.Noise,
                EnableEC = settings.EnableEC
            };
            await _dataSvc.SaveResultAsync(newRun);
            _runHistory.Add(newRun);

            statusText.Text = $"teleported! success: {success:P1}";
            UpdateChart(success);
            blochView.InvalidateSurface();
        } catch (Exception ex) {
            statusText.Text = $"oops: {ex.Message}";
            await DisplayAlert("sim error", ex.Message, "ok");
        }
    }

    private void DrawBloch(object sender, SKPaintSurfaceEventArgs e) {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        _animAngle += 0.02;

        var center = new SKPoint(125, 125);
        var radius = 100f;

        using var spherePaint = new SKPaint { Color = SKColors.LightGray, Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true };
        canvas.DrawCircle(center, radius, spherePaint);

        using var axisPaint = new SKPaint { Color = SKColors.DarkGray, StrokeWidth = 1, IsAntialias = true };
        canvas.DrawLine(center.X - radius, center.Y, center.X + radius, center.Y, axisPaint);
        canvas.DrawLine(center.X, center.Y - radius, center.X, center.Y + radius, axisPaint);

        var theta = thetaControl.Value;
        var phi = phiControl.Value;
        
        var x = Math.Sin(theta) * Math.Cos(phi);
        var y = Math.Sin(theta) * Math.Sin(phi);
        var z = Math.Cos(theta);

        var endPoint = new SKPoint(
            center.X + (float)x * radius,
            center.Y - (float)z * radius
        );

        using var vectorPaint = new SKPaint { 
            Color = SKColor.Parse("#2196F3"), 
            StrokeWidth = 4, 
            StrokeCap = SKStrokeCap.Round, 
            IsAntialias = true 
        };
        canvas.DrawLine(center, endPoint, vectorPaint);

        using var pointPaint = new SKPaint { Color = SKColor.Parse("#1976D2"), Style = SKPaintStyle.Fill, IsAntialias = true };
        canvas.DrawCircle(endPoint, 6, pointPaint);

        using var textPaint = new SKPaint { Color = SKColors.Black, TextSize = 14, IsAntialias = true, Typeface = SKTypeface.FromFamilyName("Arial", SKTypefaceStyle.Bold) };
        canvas.DrawText("|0⟩", center.X - 10, center.Y - radius - 10, textPaint);
        canvas.DrawText("|1⟩", center.X - 10, center.Y + radius + 20, textPaint);
        canvas.DrawText($"|ψ⟩ (θ={theta:F2}, φ={phi:F2})", 10, 240, textPaint);
    }

    private void UpdateChart(double success) {
        var chartData = new List<ChartEntry> {
            new ChartEntry(success * 100) { Label = "success", Color = SKColor.Parse("#4CAF50") },
            new ChartEntry((1 - success) * 100) { Label = "error", Color = SKColor.Parse("#F44336") }
        };
        successChart.Chart = new BarChart { Entries = chartData, BackgroundColor = SKColors.Transparent };
    }

    private async Task RefreshHistoryAsync() {
        _runHistory = await _dataSvc.GetHistoryAsync();
    }

    private async void ExportHistory(object sender, EventArgs e) {
        if (!_runHistory.Any()) {
            await DisplayAlert("nothing", "no runs yet.", "ok");
            return;
        }
        var exportPath = Path.Combine(FileSystem.AppDataDirectory, $"teleport_log_{DateTime.Now:yyyyMMdd}.csv");
        await _dataSvc.ExportToCsvAsync(exportPath, _runHistory);
        await DisplayAlert("saved!", $"exported to {exportPath}", "cool");
    }

    private async void ClearLog(object sender, EventArgs e) {
        await _dataSvc.ClearAllAsync();
        _runHistory.Clear();
        await DisplayAlert("cleared", "history wiped.", "ok");
    }
}
