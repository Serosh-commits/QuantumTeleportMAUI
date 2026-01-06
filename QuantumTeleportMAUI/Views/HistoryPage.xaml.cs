using QuantumTeleportMAUI.Models;
using QuantumTeleportMAUI.Services;

namespace QuantumTeleportMAUI.Views;

public partial class HistoryPage : ContentPage {
    private readonly DataService _dataSvc;

    public HistoryPage(DataService dataSvc) {
        InitializeComponent();
        _dataSvc = dataSvc;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        historyList.ItemsSource = await _dataSvc.GetHistoryAsync();
    }

    private async void GoBack(object sender, EventArgs e) {
        await Navigation.PopAsync();
    }
}
