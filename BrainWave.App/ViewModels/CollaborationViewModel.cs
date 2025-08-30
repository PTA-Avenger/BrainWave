using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.Maui.ViewModels;
public class CollaborationViewModel(ApiService api) : BaseViewModel
{
    public ObservableCollection<CollaborationModel> Items { get; } = new();
    public async Task LoadAsync()
    {
        Items.Clear(); foreach (var c in await api.GetCollaborationsAsync()) Items.Add(c);
    }
}
