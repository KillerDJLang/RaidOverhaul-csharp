using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using RaidOverhaulConfig.Models;
using RaidOverhaulConfig.ViewModels;

namespace RaidOverhaulConfig;

public partial class MainWindow : Window
{
    private static readonly JsonSerializerOptions ReadOptions = new() { PropertyNameCaseInsensitive = true };

    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = false };

    private string? _configFilePath;
    private string? _weightingsFilePath;
    private string? _originalConfigJson;
    private string? _originalWeightingsJson;

    private readonly MainViewModel _vm = new();
    private DispatcherTimer? _feedbackTimer;

    private const string ModVersion = "v3.0.2";
    private const string SptVersion = "v~4.0.x";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;

        try
        {
            LoadConfigs();
            VersionLabel.Text = $"Raid Overhaul Config  |  RO {ModVersion}  |  SPT {SptVersion}";
            ErrorPanel.Visibility = Visibility.Collapsed;
            MainTabControl.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            ErrorPanel.Visibility = Visibility.Visible;
            ErrorText.Text = $"{ex.Message}\n\nMake sure the config app is located in the root of the RaidOverhaul server folder.";
            MainTabControl.Visibility = Visibility.Collapsed;
        }
    }

    private void LoadConfigs()
    {
        var exeDir =
            Path.GetDirectoryName(Environment.ProcessPath) ?? throw new InvalidOperationException("Cannot determine executable directory.");

        _configFilePath = Path.Combine(exeDir, "config", "config.json");
        _weightingsFilePath = Path.Combine(exeDir, "config", "eventWeightings.json");

        _originalConfigJson = File.ReadAllText(_configFilePath);
        _originalWeightingsJson = File.ReadAllText(_weightingsFilePath);

        _vm.Config = JsonSerializer.Deserialize<ConfigTemplate>(_originalConfigJson, ReadOptions) ?? new ConfigTemplate();
        _vm.Weighting = JsonSerializer.Deserialize<WeightingTemplate>(_originalWeightingsJson, ReadOptions) ?? new WeightingTemplate();
    }

    private void SaveAll()
    {
        File.WriteAllText(_configFilePath!, JsonSerializer.Serialize(_vm.Config, WriteOptions));
        File.WriteAllText(_weightingsFilePath!, JsonSerializer.Serialize(_vm.Weighting, WriteOptions));
    }

    private void RevertAll()
    {
        _vm.Config = JsonSerializer.Deserialize<ConfigTemplate>(_originalConfigJson!, ReadOptions) ?? new ConfigTemplate();
        _vm.Weighting = JsonSerializer.Deserialize<WeightingTemplate>(_originalWeightingsJson!, ReadOptions) ?? new WeightingTemplate();

        File.WriteAllText(_configFilePath!, JsonSerializer.Serialize(_vm.Config, WriteOptions));
        File.WriteAllText(_weightingsFilePath!, JsonSerializer.Serialize(_vm.Weighting, WriteOptions));
    }

    private bool ValidateConfig(out string error)
    {
        var cfg = _vm.Config;

        int weatherCount = new[] { cfg.AllSeasons, cfg.NoWinter, cfg.SeasonalProgression, cfg.WinterWonderland }
            .Count(x => x);
        if (weatherCount > 1)
        {
            error = "⚠  Only one Weather & Season option can be active at a time.";
            return false;
        }

        if (cfg.BasicStackTuningEnabled && cfg.AdvancedStackTuningEnabled)
        {
            error = "⚠  Basic Stack Tuning and Advanced Stack Tuning cannot both be enabled.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateConfig(out var error))
        {
            ShowFeedback(error, (SolidColorBrush)FindResource("RevertedFg"), seconds: 4);
            return;
        }

        SaveAll();
        ShowFeedback("✓  Configuration saved!", (SolidColorBrush)FindResource("SavedFg"));
    }

    private void Revert_Click(object sender, RoutedEventArgs e)
    {
        RevertAll();
        ShowFeedback("↺  Reverted to original values!", (SolidColorBrush)FindResource("RevertedFg"));
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    private void ShowFeedback(string message, SolidColorBrush color, double seconds = 2)
    {
        StatusLabel.Foreground = color;
        StatusLabel.Text = message;

        _feedbackTimer?.Stop();
        _feedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
        _feedbackTimer.Tick += (_, _) =>
        {
            StatusLabel.Text = string.Empty;
            _feedbackTimer?.Stop();
        };
        _feedbackTimer.Start();
    }
}
