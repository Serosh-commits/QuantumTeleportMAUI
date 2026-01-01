namespace QuantumTeleportMAUI.Models;

public class TeleportRequest {
    public int NumQubits { get; set; } = 1;
    public double Theta { get; set; } = Math.PI / 3.0;
    public double Phi { get; set; } = Math.PI / 4.0;
    public int Shots { get; set; } = 1000;
    public double Noise { get; set; } = 0.0;
    public bool EnableEC { get; set; } = false;
}
