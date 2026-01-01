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
                Settings = settings
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
        canvas.Clear(SKColors.LightBlue);

        _animAngle += 0.05;

        var spherePaint = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
        canvas.DrawCircle(125, 125, 100, spherePaint);

        canvas.DrawLine(25, 125, 75, 125, spherePaint);
        canvas.DrawLine(125, 25, 125, 75, spherePaint);
        canvas.DrawLine(125, 225, 125, 175, spherePaint);

        var theta = thetaControl.Value;
        var phi = phiControl.Value;
        var vecX = Math.Sin(theta) * Math.Cos(phi);
        var vecY = Math.Sin(theta) * Math.Sin(phi);
        var vecZ = Math.Cos(theta);

        var projX = 125 + vecX * 80;
        var projY = 125 - vecZ * 80;
        var vecPaint = new SKPaint { Color = SKColors.Red, StrokeWidth = 4, PathEffect = SKPathEffect.CreateDash(new float[] { 10, 5 }, 0) };

        canvas.DrawLine(125, 125, (float)projX, (float)projY, vecPaint);

        using var textPaint = new SKPaint { Color = SKColors.Black, TextSize = 12 };
        canvas.DrawText($"|ψ⟩ at θ={theta:F1}, φ={phi:F1}", 10, 240, textPaint);
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
