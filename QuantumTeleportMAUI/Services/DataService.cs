using SQLite;
using QuantumTeleportMAUI.Models;

namespace QuantumTeleportMAUI.Services;

public class DataService {
    private readonly SQLiteAsyncConnection _db;

    private bool _initialized;

    public DataService(string dbPath) {
        _db = new SQLiteAsyncConnection(dbPath);
    }

    private async Task InitAsync() {
        if (_initialized) return;
        await _db.CreateTableAsync<ExperimentResult>();
        _initialized = true;
    }

    public async Task SaveResultAsync(ExperimentResult result) {
        await InitAsync();
        await _db.InsertAsync(result);
    }

    public async Task<List<ExperimentResult>> GetHistoryAsync() {
        await InitAsync();
        return await _db.Table<ExperimentResult>().ToListAsync();
    }

    public async Task ExportToCsvAsync(string path, List<ExperimentResult> results) {
        await Task.Run(() => {
            using var writer = new StreamWriter(path);
            writer.WriteLine("RunTime,SuccessRate,Fidelity,NumQubits,Shots,Noise,EnableEC");
            foreach (var r in results) {
                writer.WriteLine($"{r.RunTime},{r.SuccessRate},{r.Fidelity},{r.NumQubits},{r.Shots},{r.Noise},{r.EnableEC}");
            }
        });
    }

    public async Task ClearAllAsync() {
        await InitAsync();
        await _db.DeleteAllAsync<ExperimentResult>();
    }
}
