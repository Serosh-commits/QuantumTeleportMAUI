# QuantumTeleportMAUI

This is a little project I put together to experiment with quantum teleportation using .NET MAUI and Q#. Teleportation is one of the most fascinating concepts in quantum physics, and I wanted a way to visualize it and see how it holds up against noise.

It runs on Windows, Mac, Android, and iOS. The core logic is written in Q# for the actual simulation, while the UI uses SkiaSharp for the Bloch sphere visualization and Microcharts for the success rates.

### Features
- **Teleport States**: Set up any single-qubit state using polar and azimuthal angles.
- **Noise Simulation**: See what happens when you introduce decoherence into the system.
- **Error Correction**: There's a built-in 3-qubit bit-flip code you can toggle to see how it improves fidelity in noisy environments.
- **Experiment History**: All your runs are saved to a local SQLite database, and you can export them to CSV if you want to crunch the numbers elsewhere.

### Tech Stack
- **Quantum Kernel**: Microsoft Q# (QDK)
- **Framework**: .NET MAUI
- **Graphics**: SkiaSharp
- **Database**: SQLite

### How to run it
Make sure you have the .NET 8 SDK and the MAUI workload installed.

1. Clone the repo:
   ```bash
   git clone https://github.com/Serosh-commits/QuantumTeleportMAUI.git
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run for your preferred platform:
   ```bash
   dotnet run -f net8.0-windows10.0.19041.0
   # or net8.0-maccatalyst, net8.0-android, net8.0-ios
   ```

### Future Plans
- Adding multi-qubit entanglement support.
- Implementing more complex error correction like Steane or Surface codes.
- Maybe connecting it to actual hardware via Azure Quantum.

If you find any bugs or have ideas for features, feel free to open an issue or a PR.

Peace.

