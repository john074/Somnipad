using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notebook.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool _drawLines = true;
    private ThemeVariant _selectedTheme = ThemeVariant.Dark;

    public SettingsViewModel()
    {
        Application.Current!.Resources["TextBoxBorderThickness"] = new Thickness(1);
        Application.Current!.Resources["BackgroundColor"] = Colors.Black;
        Application.Current!.Resources["ForegroundColor"] = Colors.White;
        Application.Current!.Resources["BorderColor"] = Colors.Red;
        Application.Current!.Resources["SelectionColor"] = Colors.Red;
    }

    public List<ThemeVariant> Themes { get; } =
    [
        ThemeVariant.Light,
        ThemeVariant.Dark,
        new ThemeVariant("Papyrus", ThemeVariant.Light),
        new ThemeVariant("Midnight", ThemeVariant.Dark),
        new ThemeVariant("Nord", ThemeVariant.Light),
        new ThemeVariant("SolarizedDark", ThemeVariant.Dark),
        new ThemeVariant("SolarizedLight", ThemeVariant.Light),
        new ThemeVariant("Dracula", ThemeVariant.Dark),
        new ThemeVariant("GruvboxDark", ThemeVariant.Dark),
        new ThemeVariant("GruvboxLight", ThemeVariant.Light),
        new ThemeVariant("Forest", ThemeVariant.Dark),
        new ThemeVariant("Coffee", ThemeVariant.Dark),
    ];

    public bool DrawLines
    {
        get => _drawLines;
        set
        {
            if (_drawLines == value)
                return;

            _drawLines = value;
            OnPropertyChanged();

            Application.Current!.Resources["TextBoxBorderThickness"] = value ? new Thickness(1) : new Thickness(0);
        }
    }

    public ThemeVariant SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme == value)
                return;

            _selectedTheme = value;
            Application.Current!.RequestedThemeVariant = value;

            switch (_selectedTheme.Key.ToString())
            {
                case "Dark":
                    Application.Current!.Resources["BackgroundColor"] = Colors.Black;
                    Application.Current!.Resources["ForegroundColor"] = Colors.White;
                    Application.Current!.Resources["BorderColor"] = Colors.Red;
                    Application.Current!.Resources["SelectionColor"] = Colors.Red;
                    break;

                case "Light":
                    Application.Current!.Resources["BackgroundColor"] = Colors.White;
                    Application.Current!.Resources["ForegroundColor"] = Colors.Black;
                    Application.Current!.Resources["BorderColor"] = Colors.LightBlue;
                    Application.Current!.Resources["SelectionColor"] = Colors.LightBlue;
                    break;

                case "Papyrus":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#F5ECD9");
                    Application.Current!.Resources["ForegroundColor"] = Colors.Black;
                    Application.Current!.Resources["BorderColor"] = Colors.SandyBrown;
                    Application.Current!.Resources["SelectionColor"] = Colors.SandyBrown;
                    break;

                case "Midnight":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#0D1117");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#C9D1D9");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#30363D");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#58A6FF");
                    break;

                case "Nord":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#2E3440");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#ECEFF4");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#4C566A");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#88C0D0");
                    break;

                case "SolarizedDark":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#002B36");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#839496");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#586E75");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#B58900");
                    break;

                case "SolarizedLight":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#FDF6E3");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#657B83");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#93A1A1");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#268BD2");
                    break;

                case "Dracula":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#282A36");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#F8F8F2");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#44475A");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#BD93F9");
                    break;

                case "GruvboxDark":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#282828");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#EBDBB2");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#504945");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#FABD2F");
                    break;

                case "GruvboxLight":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#FBF1C7");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#3C3836");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#D5C4A1");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#D79921");
                    break;

                case "Forest":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#1B2A1F");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#DCE5D8");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#3A5A40");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#A3B18A");
                    break;

                case "Coffee":
                    Application.Current!.Resources["BackgroundColor"] = Color.Parse("#2B1E1A");
                    Application.Current!.Resources["ForegroundColor"] = Color.Parse("#E6D3B1");
                    Application.Current!.Resources["BorderColor"] = Color.Parse("#6F4E37");
                    Application.Current!.Resources["SelectionColor"] = Color.Parse("#C19A6B");
                    break;
            }

            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
