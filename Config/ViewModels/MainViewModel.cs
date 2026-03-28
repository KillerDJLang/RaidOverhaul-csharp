using System.ComponentModel;
using System.Runtime.CompilerServices;
using ROConfig.Models;

namespace ROConfig.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private ConfigTemplate _config = new();
    private WeightingTemplate _weighting = new();

    public ConfigTemplate Config
    {
        get { return _config; }
        set
        {
            _config = value;
            OnPropertyChanged();
        }
    }

    public WeightingTemplate Weighting
    {
        get { return _weighting; }
        set
        {
            _weighting = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
