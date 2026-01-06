using SQLite;
using System.ComponentModel;

namespace QuantumTeleportMAUI.Models;

public class ExperimentResult : INotifyPropertyChanged {
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    private double _successRate;
    public double SuccessRate {
        get => _successRate;
        set { _successRate = value; OnPropertyChanged(); }
    }

    public double Fidelity { get; set; } = 0.0;
    public DateTime RunTime { get; set; } = DateTime.Now;

    public int NumQubits { get; set; }
    public double Theta { get; set; }
    public double Phi { get; set; }
    public int Shots { get; set; }
    public double Noise { get; set; }
    public bool EnableEC { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
