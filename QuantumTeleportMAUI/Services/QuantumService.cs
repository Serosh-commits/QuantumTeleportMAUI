using Microsoft.Quantum.Simulation.Core;
using QuantumLib;
using QuantumTeleportMAUI.Models;

namespace QuantumTeleportMAUI.Services;

public class QuantumService {
    private readonly ILogger<QuantumService> _logger;

    public QuantumService(ILogger<QuantumService> logger) {
        _logger = logger;
    }

    public async Task<double> SimulateTeleportAsync(TeleportRequest req) {
        if (req.NumQubits < 1 || req.NumQubits > 4 || req.Shots <= 0 || req.Noise < 0 || req.Noise > 1) {
            _logger.LogWarning("bad params, using defaults");
            req.NumQubits = 1;
            req.Shots = 100;
        }

        await using var qSim = new QuantumSimulator();

        var successProb = await RunTeleportTest.Run(qSim, req.NumQubits, req.Theta, req.Phi, req.Noise, req.EnableEC, req.Shots);

        _logger.LogInformation($"sim done: {successProb:P1} success");
        return successProb;
    }

    public async Task<Dictionary<double, double>> RunBatchAsync(TeleportRequest baseReq, double[] noiseLevels) {
        var tasks = noiseLevels.Select(n => {
            var copyReq = baseReq with { Noise = n };
            return Task.Run(() => SimulateTeleportAsync(copyReq));
        }).ToArray();

        var results = await Task.WhenAll(tasks);
        var dict = new Dictionary<double, double>();
        for (int i = 0; i < noiseLevels.Length; i++) {
            dict[noiseLevels[i]] = results[i];
        }
        return dict;
    }
}
