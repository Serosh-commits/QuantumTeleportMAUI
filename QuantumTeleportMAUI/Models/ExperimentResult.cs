using System.ComponentModel;

namespace QuantumTeleportMAUI.Models;

public class ExperimentResult : INotifyPropertyChanged {
    private double _successRate;
    public double SuccessRate {
        get => _successRate;
        set { _successRate = value; OnPropertyChanged(); }
    }
    public double Fidelity { get; set; } = 0.0;
    public DateTime RunTime { get; set; } = DateTime.Now;
    public TeleportRequest Settings { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
