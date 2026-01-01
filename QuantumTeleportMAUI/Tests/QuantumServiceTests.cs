using Xunit;
using Moq;
using QuantumTeleportMAUI.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuantumTeleportMAUI.Tests;

public class QuantumServiceTests {
    private readonly Mock<ILogger<QuantumService>> _mockLogger = new();

    [Fact]
    public async Task Simulate_NoNoise_HighSuccess() {
        var svc = new QuantumService(_mockLogger.Object);
        var req = new TeleportRequest { NumQubits = 1, Shots = 50, Noise = 0.0, EnableEC = false };
        var result = await svc.SimulateTeleportAsync(req);
        Assert.True(result > 0.95);
    }

    [Fact]
    public void Simulate_BadNoise_LogsWarning() {
        var svc = new QuantumService(_mockLogger.Object);
        var req = new TeleportRequest { Noise = 2.0 };
        _ = svc.SimulateTeleportAsync(req).Result;
        _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Batch_DifferentNoises_VariesRates() {
        var svc = new QuantumService(_mockLogger.Object);
        var baseReq = new TeleportRequest { Shots = 20 };
        var noises = new[] { 0.0, 0.05 };
        var batch = await svc.RunBatchAsync(baseReq, noises);
        Assert.True(batch[0.0] > batch[0.05]);
        Assert.Equal(2, batch.Count);
    }
}
