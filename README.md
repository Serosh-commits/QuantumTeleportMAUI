# 🌌 QuantumTeleportMAUI: The Abyssal Simulator

**QuantumTeleportMAUI** is a high-fidelity, cross-platform simulation environment designed to explore the frontiers of quantum state transfer. Built on the bedrock of **.NET MAUI** and powered by a core **Q#** engine, this application provides an immersive interface for executing and analyzing quantum teleportation protocols under varying degrees of environmental decoherence.

---

## 🛰️ Core Capabilities

- **State Preparation**: Encode arbitrary single-qubit states using $(\theta, \phi)$ parameters on the Bloch Sphere.
- **Protocol Fidelity**: Execute the standard quantum teleportation circuit with high-precision Q# operations.
- **Decoherence Simulation**: Inject controlled noise levels to study the impact of depolarizing channels on teleportation success.
- **Error Mitigation**: Deploy a 3-qubit bit-flip correction code to sustain fidelity in high-noise environments.
- **Data Analytics**: Real-time visualization with SkiaSharp and Microcharts, coupled with persistent CSV logging for post-run analysis.

## 🛠️ Architecture & Tech Stack

- **Kernel**: [Microsoft QDK](https://learn.microsoft.com/quantum/) (Q#) for pure quantum logic execution.
- **Frontend**: .NET MAUI (C# / XAML) for a seamless cross-platform experience (Windows, macOS, Android, iOS).
- **Visualization**: SkiaSharp for dynamic Bloch Sphere rendering and Microcharts for statistical distribution.
- **Persistence**: SQLite (sqlite-net-pcl) for robust local storage of experimental history.

## 🚀 Execution Guide

### Prerequisites
- .NET 8.0 SDK or higher
- MAUI Workload (`dotnet workload install maui`)

### Quick Start
```bash
git clone https://github.com/Serosh-commits/QuantumTeleportMAUI.git
cd QuantumTeleportMAUI
dotnet restore
dotnet run -f [target-framework]
```
*(Targets: `net8.0-windows`, `net8.0-maccatalyst`, `net8.0-android`, `net8.0-ios`)*

## 📈 Roadmap (The Ascendance)
- [x] Persistent Experiment Logging & Export
- [x] Premium Visual Design System
- [ ] Multi-Qubit Entanglement Swapping
- [ ] Global Quantum Cloud Integration (Azure Quantum)
- [ ] Advanced Error Correction (Steane & Surface Codes)

---

Developed with passion by **Serosh**. Perfected by ARTIFICIAL INTELLIGENCE.
