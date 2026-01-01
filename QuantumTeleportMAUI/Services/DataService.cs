using SQLite;
using QuantumTeleportMAUI.Models;

namespace QuantumTeleportMAUI.Services;

public class DataService {
    private readonly SQLiteAsyncConnection _db;

    public DataService(string dbPath) {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<ExperimentResult>().Wait();
    }

    public async Task SaveResultAsync(ExperimentResult result) {
        await _db.InsertAsync(result);
    }

    public async Task<List<ExperimentResult>> GetHistoryAsync() {
        return await _db.Table<ExperimentResult>().ToListAsync();
    }

    public async Task ExportToCsvAsync(string path, List<ExperimentResult> results) {
        await Task.Run(() => {
            using var writer = new StreamWriter(path);
            writer.WriteLine("RunTime,SuccessRate,Fidelity,NumQubits,Shots,Noise,EnableEC");
            foreach (var r in results) {
                writer.WriteLine($"{r.RunTime},{r.SuccessRate},{r.Fidelity},{r.Settings.NumQubits},{r.Settings.Shots},{r.Settings.Noise},{r.Settings.EnableEC}");
            }
        });
    }

    public async Task ClearAllAsync() {
        await _db.DeleteAllAsync<ExperimentResult>();
    }
}
